using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using static System.Math;
using System.Numerics;


namespace FF1Lib.Procgen
{
    public class SpanningTree : IMapGeneratorEngine
    {
        /// uses Prim's algorithm to create a minimum spanning tree over a set of randomly chosen nodes;
        /// graph cycles are permitted with an option, and then only under very specific circumstances.
        /// The strategy is to use the randomly placed nodes as "control" nodes, and then to have the
        /// algorithm carve edges between them to make a path.
        /// While this could be generalized to other locations, the "footprint" mechanic is specialized
        /// for the 4x4-tile path element of waterfall, and the entire logic of what next step in drawing
        /// the path is legal is hardcoded for the waterfall path element.
        /// 
        /// 

        
        private Flags _flags;
        // The _pool graph contains all of the control Nodes, with weighted Edges between all pairs.
        // The weights are the squared pythagorean distance between them (with wraparound). The algorithm
        // uses weights determine which Edges to draw and which to discard (draw Edges between closer Nodes first.)
        // The number of Edges E scales with the number of Nodes N as E = N*(N-1)/2 , or "N take 2"
        private Graph _pool;

        // The _tree graph is the spanning tree among the control nodes. When cycles are allowed, it's
        // not technically a tree anymore, but in spirit it is because cycles are only allowed by drawing
        // an edge between two closely situated leaves.
        private Graph _tree;

        // The _pathNodes are the collection of points representing a 4x4 path. For lots of reasons it made
        // sense to use the Node struct for these even though they aren't connected in any graph. This could
        // be refactored to use a struct with less overhead, but this runs reasonably fast in browser.
        // Throughout the code, "Node" and "Coordinate" are used more or less interchangeably
        private List<Node> _pathNodes;

        // The first node, which sits at the entrance to the room. _tree construction begins here. This
        // node is selected as the one that has the largest minimum distance to another Node, in order
        // to make more room for the room.
        private Node _firstNode;

        // The room node, which is the coordinate of the robot room itself.
        private Node _roomNode;

        // The Footprint class maintains a set of "footprint" templates i.e. specific tile geometries,
        // and returns them to the algorithm.
        // The most important is the LocusToLocus footprint, which contains the 4x4 path element
        // and the padding around requried to ensure that no path elements from another branch of 
        // the maze encroach on this path. This relationship is reflexive.
        // A footprint is a HashSet<Node> with X,Y coordinates centered on the desired Node.
        private Footprint _footprint;

        // The _roomFootprint is a set of Nodes around the room coordinate where no path element may be drawn.
        // Any initialized control node in the _roomFootprint gets clobbered.
        private HashSet<Node> _roomFootprint;

        // If the closest control node to _firstNode is to the right, we flip the room horizontally
        // and put the door and spiketile on the right.
        private bool _flipRoom = false;

        // The NextStep class maintains the logic for drawing the path. Essentially, it takes a current location
        // and a target, and returns a list of coordinates that can be the next step, weighted both by the vector
        // direction from current to target, and by the overlap between this path element and the next. In general
        // there are 80 tiles about the current one that could overlap in the right way.
        // The aesthetics of the original waterfall favors a 3-4 tile overlap between path elements, so the weighting
        // favors middle-distance steps. The direction vector weighting is specified as a tolerance in degrees,
        // e.g. if the tolerance is 45 degrees, we're going to consider any tile that is within 45 degrees in either
        // direction from the direction vector to the target.
        private NextStep _nextStep;

        // A list of the initial control nodes on which the _pool Graph is built.
        private List<Node> _initNodes;

        // VERY rarely, the next Node after _firstNode is in a weird place that the path drawing routine can't
        // get to legally without stepping into the _roomFootprint. It's easier to return a null map and reroll
        // at this point than to redo the geometry of the room and firstnode.
        private bool _insane = false;

        // The step tolerance in degrees. This will be different for different settings. In general, the denser
        // the map, the less tolerance. Sparser maps get to meander beautifully in all that empty space.
        private int _stepTolerance;

        // The avoid loops flag
        private bool _avoidLoops;

        // How far away can two leaves on the tree be in order to join them into a loop?
        private int _maxLoopDistance;

        // if we're using long hallway, we need to be able to draw the hallway
        // independent of other Nodes; the _lineGraph is an independent Graph
        // of all the hallway nodes.
        private bool _hallway = false;
        private Graph _lineGraph;

        ////////////////////////////////////////////////////////////////////////////////////
        // Constructor
        public SpanningTree(Flags flags)
        {
            _flags = flags;
            _avoidLoops = (bool)flags.ProcgenWaterfallNoLoops;
        }


        //////////////////////////////////////////////////////////////////
        /// Generate() runs all the steps, and contains the primary loop for Prim's algorithm.
        /// Also does the tile continuity drawing.
        public CompleteMap Generate(MT19337 rng, MapRequirements reqs)
        {
            _footprint = new();
            _initNodes = new();
            _nextStep = new();
            _tree = new();
            _pathNodes = new();
            _lineGraph = new();

            // This is the waterfall path element.
            byte[,] Path =
            {
                // (byte)Tile.WaterfallRandomEncounters == 0x49
                {0x49,0x49,0x49,0x49},
                {0x49,0x49,0x49,0x49},
                {0x49,0x49,0x49,0x49},
                {0x49,0x49,0x49,0x49}
            };
            
            // Initialize the _pool graph of control Nodes. Most of what the end result looks like takes place here.
            InitPoolGraph(rng);
            // Console.WriteLine($"Pool graph has {_pool.Nodes.Count} Nodes and {_pool.Edges.Count} Edges.");

            // We could use a global priority queue for this, but it was running into performance issues.
            // The algorithm works by dumping a Node's adjacent edges into the priority queue when it is attached
            // to the tree, and then pulling the lowest-weight edge from the queue to attach the next node.
            // Each time this happens, the queue has to heapify. This doesn't take TOO much time, but it was
            // annoying enough.
            // PriorityQueue<Edge,int> edgeQueue = new();

            // Instead, here is a MinimumEdgeQueue class that maintains a separate PriorityQueue for each Node,
            // initialized with all of its edges. It also contains a list of nodes it's allowed to dequeue edges for.
            // Since we don't know the order that Nodes are attached to the tree, we will actually process every Edge
            // twice, once from the POV of each Node at the end of the Edge. There's no good way to prune these ahead of time
            // unless we are also pruning the _pool, but the latter has way more overhead than just discarding Edges that are
            // already in the _tree.
            MinimumEdgeQueue edgeQueue = new(_pool.Adjacencies);
            // we also have a separate queue for drawing the hallway. It will be empty if we're in another mode
            MinimumEdgeQueue lineQueue = new(_lineGraph.Adjacencies);

            // onLine is a flag which tells us whether or not we are building the hallway
            bool onLine = false;

            /////////////////////////////////////////////////////////////
            /// Prim's algorithm begins here.
            /// The basic steps are:
            /// 1. Add the first two Nodes and the Edge between them to the _tree
            /// 2. Store the coordinates of the path carved by that edge
            /// 3. Construct a total footprint taken up by that path, where new paths elements can't be placed.
            /// 4. Get the next edge from the edgeQueue
            /// 5. Try to construct a path from the current node to the next one; if you can't do it without
            ///    colliding with another path, move on to the next edge.
            /// 6. If 5. succeeds, add the Edge to the tree, store this path's set of coordinates,
            ///    add this path's footprint to the total footprint.
            /// 7. Loop 4. 5. 6. until we've attached all the nodes, or until the edgeQueue is empty, or
            ///    we've reached 10,000 total loops.

            Node thisNode;
            Edge firstEdge;
            /// these are the resulting pathNodes in steps 2. 5. and 6.
            List<Node> resultingNodes;

            /// the set of coordinates we can no longer draw a path element at.
            HashSet<Node> totalFootprint;


            /////////////////////////////////////////////////////////////////////
            /// STEP 1.
            /// Add the first two Nodes and the Edge between them to the _tree

            // first Node-to-Node path
            (thisNode, firstEdge, resultingNodes, totalFootprint) = FirstPath(rng);
            /// bail if something went wrong.
            /// This includes if the _firstNode is on the hallway line but not the second one
            if (_insane || (_avoidLoops && _hallway && _lineGraph.Contains(_firstNode) && !_lineGraph.Contains(thisNode)))
            {
                return null;
            }
            // if the firstEdge happened to be between two Nodes in the long hallway,
            // we'll continue to draw the long hallway.
            if (_hallway && _lineGraph.Contains(firstEdge))
            {
                onLine = true;
            }

            _tree.Add(firstEdge);

            ///////////////////////////////////////////////////////////
            /// STEP 2.
            /// Store the coordinates of the path carved by that edge
            _pathNodes.AddRange(resultingNodes);

            // If we aren't allowing loops, we want the room to be at a leaf, because there
            // is only one path from the entrance to the room, and anything beyond the room
            // would be wasted space. 
            if (!_avoidLoops)
            {
                edgeQueue.IncreaseScope(_firstNode);
                if (onLine)
                {   
                    lineQueue.IncreaseScope(_firstNode);
                }
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////
            /// STEP 3.
            /// Construct a total footprint taken up by that path, where new path elements can't be placed.
            totalFootprint.UnionWith(_roomFootprint);

            /// edgeQueue can now return edges from thisNode
            edgeQueue.IncreaseScope(thisNode);
            if (onLine)
            {  
                    lineQueue.IncreaseScope(thisNode);   
            }


            // loop counter
            int n = 0;
            // number of Nodes placed so far
            int doneNodes = 2;
            // total number of Nodes in the _pool
            int numNodes = _pool.Nodes.Count;

            ////////////////////////////////////////
            /// MAIN LOOP FOR STEPS 4. 5. & 6.
            while (edgeQueue.Count() > 0 && doneNodes != (_avoidLoops ? numNodes : numNodes + 10) && n<10000)
            {
                // Console.Write($"Loop Number: {n}  Priority Queue Length: {edgeQueue.Count()}  \r");
                n++;
                Node targetNode;
                Node? nextNode;
                HashSet<Node> thisFootprint;
                HashSet<Node> resultingFootprint;

                // if we have been drawing the hallway, but the lineQueue is empty,
                // we need to move on to the other Nodes
                if (onLine && lineQueue.Count() == 0)
                {
                    onLine = false;
                }

                ///////////////////////////////////////////
                /// STEP 4.
                /// Get the next edge from the edgeQueue or lineQueue
                Edge candidateEdge;
                if (onLine)
                {
                    candidateEdge = lineQueue.Dequeue();
                }
                else
                {
                    candidateEdge = edgeQueue.Dequeue();
                    _lineGraph.Remove(candidateEdge);
                }

                Node node1 = candidateEdge.Node1;
                Node node2 = candidateEdge.Node2;
                
                /// If both nodes are already attached, then this edge would make
                /// a loop. If we're avoiding loops, we can just continue to the next
                /// iteration
                if (_tree.Contains(node1) && _tree.Contains(node2))
                {
                    if (_avoidLoops)
                    {
                        continue;
                    }
                    else
                    {
                        // otherwise, we might want to allow a loop.
                        // The following are exclusion criteria.
                        // discard if we have already drawn this edge
                        if(_tree.Contains(candidateEdge)
                            //
                            || candidateEdge.Weight > _maxLoopDistance * _maxLoopDistance
                            // discard if either node is not currently a leaf of the tree
                            || _tree.Adjacencies[node1].Count > 1
                            || _tree.Adjacencies[node2].Count > 1 
                            // flip a coin. This could also be tuned to respond to initial distribution
                            || rng.Between(0,1) == 0)
                        {
                            continue;
                        }
                    }
                }

                // if we made it this far, it's time to try to draw the edge. First we need to know
                // which of the Edge's nodes is already in the tree.
                if (_tree.Contains(candidateEdge.Node1))
                {
                    thisNode = candidateEdge.Node1;
                    targetNode = candidateEdge.Node2;
                }
                else
                {
                    thisNode = candidateEdge.Node2;
                    targetNode = candidateEdge.Node1;
                }

                /// if we're allowing loops, we don't care about path collisions, so we only look to see
                /// if the targetNode is in the _roomFootprint. This might be extraneous...
                /// Otherwise, if the targetNode is in the the total footprint, we just ignore this Node
                if (_roomFootprint.Contains(targetNode) || (totalFootprint.Contains(targetNode) && _avoidLoops))
                {
                    continue;
                }

                // get a copy of the total footprint
                thisFootprint = totalFootprint.ToHashSet();
                // since we want to draw from this node, we need to remove its footprint
                // from the forbidden coordinates...
                thisFootprint.ExceptWith(_footprint.LocusToLocus(thisNode));
                // ...but this might have removed some coordinates that were part of the _roomFootprint,
                // which are always forbidden, so we need to add them in just in case.
                thisFootprint.UnionWith(_roomFootprint);

                /////////////////////////////////////////////////////////////////////////////////////////////
                /// STEP 5.
                /// Try to construct a path from the current node to the next one; if you can't do it without
                /// colliding with another path, move on to the next edge.
                (nextNode, resultingNodes,resultingFootprint) = NextPath(thisNode,targetNode,thisFootprint,rng);   

                /// bail on this if the path couldn't complete
                if (nextNode != targetNode)
                {
                    continue;
                }

                if (_hallway && _lineGraph.Contains((Node)nextNode))
                {
                    onLine = true;
                }

                doneNodes++;

                // Console.WriteLine($"\r\nPlaced {doneNodes} Nodes in map.");
                

                ////////////////////////////////////////////////////////////////////////////////////
                /// STEP 6.
                /// If 5. succeeds, add the Edge to the tree, store this path's set of coordinates,
                /// add this path's footprint to the total footprint.
                _tree.Add(candidateEdge);
                _pathNodes.AddRange(resultingNodes);
                
                totalFootprint.UnionWith(resultingFootprint);
                
                // if (_pool.Adjacencies.ContainsKey(targetNode))
                // {
                
                edgeQueue.IncreaseScope(targetNode);
                if (onLine)
                {   
                    lineQueue.IncreaseScope(targetNode);
                }
                // }

                ////////////////////////////////
                /// Step 7.
                /// Loop!   
            }
            

            ////////////////////////////////////
            /// Now we have the info we need to draw the actual map
            /// See MapGenerator.cs for the requirements detail.
            CompleteMap cm = new()
            {
                // initially fill the map with nothing but wall.
                // we'll excavate the path from there
                Map = new((byte)Tile.WaterfallInside),
                Requirements = reqs
            };

            /// Excavate the path!
            foreach (Node node in _pathNodes)
            {
                cm.Map.Put(node.XY, Path);
            }           


            /// This is a smoothing step. We're going to remove a set of tile formations that can't
            /// legally be drawn. VERY rarely this can erode a loop between two paths.
            /// We run this until we make it through cleanly without changing the map.
            /// This just replaces wall with path if any horizontal stretch of wall is less than 2 tiles
            /// between path tiles, or any vertical stretch is less than 3 tiles between path tiles.
            bool unsmoothed = true;

            while (unsmoothed)
            {
                unsmoothed = false;
                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        // Tile.WaterfallRandomEncounters = 0x49
                        if (cm.Map[y,x] != (byte)Tile.WaterfallRandomEncounters)
                        {
                            /// remove any single-tile span of wall between path tiles
                            if ((cm.Map[y-1,x] == 0x49 && cm.Map[y+1,x] == 0x49) 
                             || (cm.Map[y,x-1] == 0x49 && cm.Map[y,x+1] == 0x49))
                            {
                                cm.Map[y,x] = 0x49;
                                unsmoothed = true;
                            }
                            /// remove any double-tile of wall between path tiles in the y dimension
                            if (cm.Map[y-1,x] == 0x49 && cm.Map[y+1,x] != 0x49 && cm.Map[y+2,x] == 0x49)
                            {
                                cm.Map[y,x] = 0x49;
                                cm.Map[y+1,x] = 0x49;
                                unsmoothed = true;
                            }
                        }
                    }
                }
                
            }

            // Draw the visible wall tiles at the top boundary of the path.
            // We also need a wall tile in the event that two walls touch by just one tile edge in the y direction, like this:
            // ==============\
            //               |
            // ______________/
            // HHHHHHHHHHHHHHH <- this wall tile does not sit above a path tile
            //               /=============
            //               |
            //               \_____________
            //               HHHHHHHHHHHHHH
            //
            // However, if we leave it there, once in a while we'll get this:
            //       /=======\
            //       |       |
            //   -> \________/
            //      HHHHHHHHHH
            // =====\        /=============
            // _____/     -> H\____________  these tiles are discontinuous with the one above, so we delete both
            // HHHHHH         HHHHHHHHHHHHH
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    // Tile.WaterfallInside == 0x46;
                    if (cm.Map[y,x] == (byte)Tile.WaterfallInside)
                    {
                        if (cm.Map[y+1,x] == (byte)Tile.WaterfallRandomEncounters
                            || (cm.Map[y,x+1] == (byte)Tile.WaterfallRandomEncounters && cm.Map[y+1,x-1] == (byte)Tile.WaterfallRandomEncounters)
                            || (cm.Map[y,x-1] == (byte)Tile.WaterfallRandomEncounters && cm.Map[y+1,x+1] == (byte)Tile.WaterfallRandomEncounters)
                        )
                        {
                            cm.Map[y,x] = (byte)Tile.InsideWall; //0x30
                            if (cm.Map[y+3,x] == (byte)Tile.WaterfallRandomEncounters)
                            {
                                cm.Map[y+1,x] = (byte)Tile.WaterfallRandomEncounters;
                                cm.Map[y+2,x] = (byte)Tile.WaterfallRandomEncounters;
                            }
                            if (cm.Map[y-2,x] == (byte)Tile.WaterfallRandomEncounters)
                            {
                                cm.Map[y  ,x] = (byte)Tile.WaterfallRandomEncounters;
                                cm.Map[y-1,x] = (byte)Tile.WaterfallRandomEncounters;
                            }
                        }
                    }
                }
            }

            /// Add all the front wall outlines
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    if (cm.Map[y+1,x] == (byte)Tile.InsideWall)
                    {
                        if (cm.Map[y,x-1] == (byte)Tile.WaterfallRandomEncounters
                        || cm.Map[y,x-1] == (byte)Tile.InsideWall)
                        {
                            cm.Map[y,x] = (byte)Tile.RoomFrontLeft;
                        }
                        else if (cm.Map[y,x+1] == (byte)Tile.WaterfallRandomEncounters
                            || cm.Map[y,x+1] == (byte)Tile.InsideWall)
                        {
                            cm.Map[y,x] = (byte)Tile.RoomFrontRight;
                        }
                        else
                        {
                            cm.Map[y,x] = (byte)Tile.RoomFrontCenter;
                        }
                    }
                }
            }

            //// add all the back wall outlines
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    if (cm.Map[y,x] == (byte)Tile.WaterfallInside
                        && (cm.Map[y-1,x] == (byte)Tile.WaterfallRandomEncounters
                            || cm.Map[y-1,x] == (byte)Tile.InsideWall)
                    )
                    {
                        if (cm.Map[y,x-1] == (byte)Tile.WaterfallRandomEncounters)
                        {
                            cm.Map[y,x] = (byte)Tile.RoomBackLeft;
                        }
                        else if (cm.Map[y,x+1] == (byte)Tile.WaterfallRandomEncounters)
                        {
                            cm.Map[y,x] = (byte)Tile.RoomBackRight;
                        }
                        else
                        {
                            cm.Map[y,x] = (byte)Tile.RoomBackCenter;
                        }
                    }
                }
            }

            /// add all the left and right wall outlines
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    if (cm.Map[y,x] == (byte)Tile.WaterfallInside)
                    {
                        if (cm.Map[y,x-1] == (byte)Tile.WaterfallRandomEncounters
                        || cm.Map[y,x-1] == (byte)Tile.InsideWall)
                        {
                            cm.Map[y,x] = (byte)Tile.RoomLeft;
                        }
                        if (cm.Map[y,x+1] == (byte)Tile.WaterfallRandomEncounters
                            || cm.Map[y,x+1] == (byte)Tile.InsideWall)
                        {
                            cm.Map[y,x] = (byte)Tile.RoomRight;
                        }
                    }
                }
            }

            /// these are in MapGenerator.cs
            var room = reqs.Rooms.Single();
            var robot = room.NPCs.Single();

            /// place the robot room and the bottom row of tiles depending on whether we need to flip the room
            cm.Map.Put(_roomNode.XY,room.Tiledata);
            if (_flipRoom)
            {
                robot.Coord.x = room.Width - robot.Coord.x;
                cm.Map.Put((_roomNode.X, _roomNode.Y + room.Height), new byte[,] {{ 0x01, 0x01, 0x01, 0x02, 0x30, 0x36, 0x30, 0x30 }});
            }
            else
            {
                cm.Map.Put((_roomNode.X, _roomNode.Y + room.Height), new byte[,] {{ 0x30, 0x30, 0x36, 0x30, 0x00, 0x01, 0x01, 0x01 }});
            }
            // place the spike tile and the doorway exit tile above and below the door
            for (int x = _roomNode.X; x < _roomNode.X+room.Width; x++)
            {
                int y = _roomNode.Y + room.Height;
                if (cm.Map[y,x] == (byte)Tile.Door)
                    {
                        cm.Map[y+1,x] = (byte)Tile.Doorway;
                        cm.Map[y-1,x] = (byte)Tile.WaterfallSpikeTile;
                    }
            }

            // place the robot in the room
            robot.Coord.x += _roomNode.X;
            robot.Coord.y += _roomNode.Y;
            reqs.MapObjects.SetNpc(robot.Index, robot.ObjectId, robot.Coord.x, robot.Coord.y, robot.InRoom, robot.Stationary);

            // bats and out-of-bounds bat are placed randomly, in MapGenerator.cs

            // get the entrance location
            Node entranceNode = GetEntranceNode(rng);

            // place it in the map
            cm.Map[entranceNode.Y+2,entranceNode.X+2] = (byte)Tile.WarpUp;

            // and update the entrance pointer so you spawn on the WarpUp stairway.
            cm.Entrance = new((byte)(entranceNode.X+2),(byte)(entranceNode.Y+2),CoordinateLocale.Standard);

            // Console.Write(cm.AsText());
            if (_flags.ProcgenWaterfallSpoiler)
            {
                Utilities.ProcgenWaterfallCache = cm.AsText();
            }
            return cm;
        } /////////////////////////
        //////END Generate()
        

        /// Initialize the _pool graph according to the flag settings.
        private void InitPoolGraph(MT19337 rng)
        {
            // this is to be able to choose a random function if desired. This could be
            // done with goto case in the switch instead of a delegate, but I hate goto's, so..
            Func<MT19337,Graph> InitDistribution;
            switch (_flags.ProcgenWaterfall)
            {
                case ProcgenWaterfallMode.Uniform :
                    {
                        InitDistribution = UniformRandom;
                        break;
                    }
                case ProcgenWaterfallMode.Polar :
                    {
                        InitDistribution = PolarRandom;
                        break;
                    }
                case ProcgenWaterfallMode.JitteredEven :
                    {
                        InitDistribution = JitteredEvenRandom;
                        break;
                    }
                case ProcgenWaterfallMode.Linear :
                    {
                        InitDistribution = LinearRandom;
                        break;
                    }
                // case ProcgenWaterfallMode.Random:
                default:
                    {
                        InitDistribution = new List<Func<MT19337,Graph>>() {UniformRandom, PolarRandom, JitteredEvenRandom, LinearRandom}.PickRandom(rng);
                        break;
                    }
            }            
            _pool = InitDistribution(rng);
        }

        // Get a Graph of nodes chosen randomly from the 64x64, spaced so that a full wall can be
        // drawn around each node. We hope the distribution ends up a little clumpy
        private Graph UniformRandom(MT19337 rng)
        {
            int numNodes;
            //int numNodes = rng.Between(27,37);
            switch (_flags.ProcgenWaterfallDensity)
            {
                case ProcgenWaterfallDensity.Sparse:
                {
                    numNodes = rng.Between(13,23);
                    _stepTolerance = 75;
                    _maxLoopDistance = 20;
                    break;
                }
                case ProcgenWaterfallDensity.Normal:
                {
                    numNodes = rng.Between(27,37);
                    _stepTolerance = 45;
                    _maxLoopDistance = 18;
                    break;
                }
                case ProcgenWaterfallDensity.Dense:
                {
                    numNodes = rng.Between(45,55);
                    _stepTolerance = 15;
                    _maxLoopDistance = 16;
                    break;
                }
                default:
                {
                    _stepTolerance = 45;
                    _maxLoopDistance = 20;
                    numNodes = 32;
                    break;
                }
            }
            HashSet<Node> totalFootprint = new();
            int n = 0;
            // there could be lots of misses here, but in testing it usually only needs a few
            // extra loops to get the target number of nodes
            while (_initNodes.Count < numNodes)
            {
                Node node = new(rng.Between(0,63),rng.Between(0,63));
                if (!totalFootprint.Contains(node))
                {
                    _initNodes.Add(node);
                    totalFootprint.UnionWith(_footprint.LocusToLocus(node));
                }
                n++;
            }
            // Console.WriteLine($"Initialized {_initNodes.Count} Uniform Random nodes in {n} loops.");
            return new Graph(_initNodes);
        }

        // Get a graph of nodes chosen a random angle from the center, and a random distance
        // weighted towards the center. This creates a more compact map, with most of the path
        // near the center and branches on the periphery.
        private Graph PolarRandom(MT19337 rng)
        {
            int numNodes;
            // "pow" is a power used below to weight random distances to the center.
            double pow;
            int width;
            switch (_flags.ProcgenWaterfallDensity)
            {
                case ProcgenWaterfallDensity.Sparse:
                {
                    numNodes = rng.Between(13,23);
                    width = 2800;
                    pow = _avoidLoops ? 2.0 : 1.5;
                    _stepTolerance = 75;
                    _maxLoopDistance = 20;
                    break;
                }
                case ProcgenWaterfallDensity.Normal:
                {
                    numNodes = rng.Between(27,37);
                    width = 3000;
                    pow = _avoidLoops? 3.0 : 2.5;
                    _stepTolerance = 45;
                    _maxLoopDistance = 18;
                    break;
                }
                case ProcgenWaterfallDensity.Dense:
                {
                    numNodes = rng.Between(45, 55);
                    width = 3200;
                    pow = _avoidLoops? 4.0 : 3.5;
                    _stepTolerance = 15;
                    _maxLoopDistance = 16;
                    break;
                }
                default:
                {
                    width = 3200;
                    pow = 2;
                    numNodes = 32;
                    _stepTolerance = 75;
                    _maxLoopDistance = 20;
                    break;
                }
            }
            HashSet<Node> totalFootprint = new();
  
            const double degToRad = PI/180.0;
            int n = 0;

            while (_initNodes.Count < numNodes && n < 1000)
            {
                /// get random degrees
                int theta = rng.Between(0,359);
                /// get a random distance (actually 100*tile distance)
                int dist = rng.Between(0,width);
                // scale so that it's more concentrated towards the center
                double scaledDist = width*Pow((double)dist/width,pow);
                // convert polar to rectangular, translate to center of map, and scale back down
                int X = (int)Round((3200.0 + scaledDist*Cos(theta * degToRad))/100.0);
                int Y = (int)Round((3200.0 + scaledDist*Sin(theta * degToRad))/100.0);

                Node node = new(X,Y);
                if (!totalFootprint.Contains(node))
                {
                    _initNodes.Add(node);
                    if (_avoidLoops)
                    {
                        totalFootprint.UnionWith(_footprint.LocusToLocus(node));
                    }
                    else
                    /// if we're allowing loops, we use the opportunity to force a few more
                    /// Nodes towards the center
                    {
                        totalFootprint.UnionWith(_footprint.SmallLocusToLocus(node));
                    }
                }
                n++;
            }
            // Console.WriteLine($"Initialized {_initNodes.Count} Polar Random nodes in {n} loops.");       
            return new Graph(_initNodes);
        }

        // this produces an even distribution of Nodes, and then moves them a short distance away
        private Graph JitteredEvenRandom(MT19337 rng)
        {
            // size of the area over which we can move a Node
            int kernelWidth;
            // How to divide the map to distribute the nodes evenly
            int divisor;
            switch (_flags.ProcgenWaterfallDensity)
            {
                /// always produces 18 Nodes
                case ProcgenWaterfallDensity.Sparse :
                {
                    divisor = 3;
                    kernelWidth = 7;
                    _stepTolerance = 60;
                    _maxLoopDistance = 20;
                    break;
                }
                /// always produces 32 Nodes
                case ProcgenWaterfallDensity.Normal :
                {
                    divisor = 4;
                    kernelWidth = 5;
                    _stepTolerance = 45;
                    _maxLoopDistance = 18;
                    break;
                }
                /// always produces 50 Nodes
                case ProcgenWaterfallDensity.Dense :
                {
                    divisor = 5;
                    kernelWidth = 3;
                    _stepTolerance = 30;
                    _maxLoopDistance = 16;
                    break;
                }
                default:
                {
                    divisor = 4;
                    kernelWidth = 5;
                    _stepTolerance = 60;
                    _maxLoopDistance = 20;
                    break;
                }
            }

            // flip a coin to determine whether we jitter over a uniform distribution about the center
            // or a normal (Gaussian) distribution about the center. The normal distribution has the
            // same range as uniform, but is weighted towards the center, making for less overall variance.
            // The uniform distribution will produce a less evenly spaced map, while the normal distribution
            // will be very even, and the small jitter just adjusts the weights between Nodes for drawing Edges
            bool normalWeighting = rng.Between(0,1) == 0;

            List<Node> kernel = new();
            int halfWidth = kernelWidth/2;
            
            // build a set of Nodes centered on (0,0).
            for (int y = 0; y < kernelWidth; y++)
            {
                for (int x = 0; x < kernelWidth; x++)
                {
                    kernel.Add(new(x-halfWidth,y-halfWidth));
                }
            }

            // build the normal weighting
            List<int> normalWeights = new();

            if (normalWeighting)
            {
                Console.WriteLine("Normal");
                Node kernelCenter = new(0,0);
                List<int> SqDists = kernel.Select(node => kernelCenter.SqDist(node)).ToList();
                double rDoubleVariance = 1.0 /(2.0 * SqDists.Sum()/SqDists.Count);

                foreach (int sd in SqDists)
                {
                    double weight = Exp(-sd*rDoubleVariance)/SqDists.Count(i => i == sd);
                    normalWeights.Add((int)Round(weight * 10000.0));
                }
            }

            /// build the even distribution
            for (int i = 0; i < divisor*divisor; i++)
            {
                int x = (i*64 / divisor) % 64;
                int x2 = x + 32/divisor;
                int y = (i/divisor)*64/divisor;
                int y2 = y + 32/divisor;

                (int,int)[] centers = {(x,y),(x2,y2)};

                foreach ((int,int) coord in centers)
                {
                    if (normalWeighting)
                    {
                        _initNodes.Add(kernel.Select(i => i + coord).Zip(normalWeights).ToList().PickRandomItemWeighted(rng));
                    }
                    else
                    {
                        _initNodes.Add(kernel.Select(i => i + coord).ToList().PickRandom(rng));
                    }
                }

                /// the distribution before jitter looks like this
                /// .   .   .   .
                ///   .   .   .   .
                /// .   .   .   .
                ///   .   .   .   .
                /// .   .   .   .
                ///   .   .   .   .
                /// .   .   .   .
                ///   .   .   .   .
            }

            // Console.WriteLine($"Initialized {_initNodes.Count} Jittered Random nodes. {normalWeighting}");
            return new Graph(_initNodes);
        }

        // builds the long hallway
        private Graph LinearRandom(MT19337 rng)
        {
            _hallway = true;
            HashSet<Node> totalFootprint = new();
            // number of Nodes to try to place after the hallway is drawn
            int numNodes;
            int theta;
            int hallwayLength;
            /// theta is in degrees * 100 -- gives us more angle precision
            const double degToRad = PI/18000.0;
            // the hallway Nodes
            List<Node> line = new();
            bool sparse = false;
            bool normal = false;
            bool dense = false;
            switch (_flags.ProcgenWaterfallDensity)
            {
                case ProcgenWaterfallDensity.Sparse:
                {
                    sparse = true;
                    numNodes = rng.Between(8,18);
                    break;
                }
                case ProcgenWaterfallDensity.Normal:
                {
                    normal = true;
                    numNodes = rng.Between(19,29);
                    break;
                }
                case ProcgenWaterfallDensity.Dense:
                {
                    dense = true;
                    numNodes = rng.Between(30,40);   
                    break;
                }
                default:
                {
                    normal = true;
                    numNodes = 30;
                    break;
                }
            }

            switch (_flags.ProcgenWaterfallHallwayLength)
            {
                // 0.75 to 1.5 maplengths
                case ProcgenWaterfallHallwayLength.Short:
                {
                    hallwayLength = rng.Between(48,95);
                    if (sparse)
                        {
                            _stepTolerance = 75;
                            _maxLoopDistance = 20;
                        }
                    if (normal)
                        {
                            _stepTolerance = 60;
                            _maxLoopDistance = 18;
                        }
                    if (dense)
                        {
                            _stepTolerance = 45;
                            _maxLoopDistance = 16;
                        }
                    break;
                }
                // each next tier spans 64 steps
                // can't go the full 320 steps or the whole
                // space can be saturated by one long hallway
                //  1.5 to ca. 2.5 maplengths
                case ProcgenWaterfallHallwayLength.Mid:
                {
                    hallwayLength = rng.Between(96,159);
                    if (sparse)
                        {
                            _stepTolerance = 50;
                            _maxLoopDistance = 16;
                        }
                    if (normal)
                        {
                            _stepTolerance = 35;
                            _maxLoopDistance = 14;
                        }
                    if (dense)
                        {
                            _stepTolerance = 20;
                            _maxLoopDistance = 12;
                        }
                    break;
                }
                // ca. 2.5 to ca. 4.0 maplengths
                case ProcgenWaterfallHallwayLength.Long:
                {
                    _stepTolerance = 20;
                    _maxLoopDistance = 12;
                    hallwayLength = rng.Between(160,223);
                    break;
                }
                // ca 4.0-5.0 maplengths!!!
                case ProcgenWaterfallHallwayLength.Absurd:
                {
                    _stepTolerance = 20;
                    _maxLoopDistance = 12;
                    hallwayLength = rng.Between(224,287);
                    break;
                }
                default:
                {
                    _stepTolerance = 60;
                    _maxLoopDistance = 20;
                    hallwayLength = 64;
                    break; 
                }
            }

            // here we need to choose a theta that lets us wrap the requisite number of times.
            // moreover, it's better if the hallway wraps aren't extremely visible from the hallways
            // These values are actually theta * 100 for more precision; the conversion
            // to radians rescales them.

            if (hallwayLength < 64)
            {
                theta = rng.Between(0,9000);
            }
            else if (hallwayLength < 128)
            {
                theta = new List<int>
                {
                    // angles ensure that the next stretch of hallway are at least 11 tiles away
                    // vertically or 12 tiles away horizontally -- this is the visible distance
                    // from mapman + 4 (the width of the path element)
                    rng.Between(975,3962),
                    rng.Between(5090,8024),
                }
                .PickRandom(rng);
            }
            else if (hallwayLength < 192)
            {
                theta = new List<int>
                {
                    // now we need to ensure we can fit up to 3 lengths; the breakpoints are thus
                    // atan(1/3), atan(2/3), atan(3/2), atan(3)
                    rng.Between(975,1843),
                    rng.Between(3369,3962),
                    rng.Between(5090,5630),
                    rng.Between(7156,8024)
                }
                .PickRandom(rng);
            }
            else if (hallwayLength < 256)
            {
                // up to 4 lengths
                // atan(1/4), atan(3/4), atan(4/3), atan(4)
                theta = new List<int>
                {
                    rng.Between(975,1403),
                    rng.Between(3686,3962),
                    rng.Between(5090,5313),
                    rng.Between(7596,8024)
                }
                .PickRandom(rng);
            }
            else
                // up to 5 lengths
                // atan(1/5), atan(4/5), atan(5/4), atan(5)
            {
                theta = new List<int>
                {
                    rng.Between(975,1130),
                    rng.Between(3866,3962),
                    rng.Between(5090,5134),
                    rng.Between(7869,8024)
                }
                .PickRandom(rng); 
            }

            // use hallway length parity to make theta positive or negative, and renormalize to the 0-180.00 degree range
            // we could just flip a coin but the overall hallway length is imperceptibly even or odd
            theta *= ((hallwayLength % 2) * 2 - 1);
            theta += 18000;
            theta %= 18000;

            /// now we need to draw the line. If the absolute value of the slope is less than 1,
            /// we iterate over x and draw the corresponding y value. If it is greater than 1,
            /// we iterate over y and draw the corresponding x value. We also need to space them adequately
            /// so that the Graph is not too large (remember that the number of Edges is n take 2).
            /// We could done the one iteration and then swapped x,y for the other slope condition,
            /// but this literal code portrays the line drawing more clearly.
            if (theta <= 4500 || theta >= 13500 )
            {
                double slope = Tan(theta * degToRad);
                // this starts the hallway in a corner for easier visualization in the spoiler
                double y = slope < 0 ? 63 : 0;
                Node thisNode = new(0,(int)y);
                _initNodes.Add(thisNode);
                totalFootprint = _footprint.LocusToLocus(thisNode);

                for (int x = 0; x < hallwayLength; x++, y+=slope)
                {
                    Node nextNode = new(x,(int)Round(y));
                    if (!totalFootprint.Contains(nextNode))
                    {
                        _initNodes.Add(nextNode);
                        totalFootprint.UnionWith(_footprint.LocusToLocus(nextNode));
                        _lineGraph.Add(new Edge(thisNode,nextNode));
                        thisNode = nextNode;
                    }
                }
            }
            else
            {
                // if the absolute value of the actual slope is greater than 1.0, we iterate over y
                // and use the reciprocal slope to get corresponding x values.
                // Tan(90) is infinite/undefined, so we'll go ahead and just declare 1/Tan(90) to be 0;
                double rSlope = theta == 9000? 0 : 1.0/Tan(theta * degToRad);
                double x = rSlope < 0 ? 63 : 0;
                // Console.WriteLine($"Slope: {slope}");
                Node thisNode = new((int)x,0);
                _initNodes.Add(thisNode);
                totalFootprint = _footprint.LocusToLocus(thisNode);
                for (int y = 0; y < hallwayLength; y++, x+=rSlope)
                {
                    Node nextNode = new((int)Round(x),y);
                    if (!totalFootprint.Contains(nextNode))
                    {
                        _initNodes.Add(nextNode);   
                        totalFootprint.UnionWith(_footprint.LocusToLocus(nextNode));
                        _lineGraph.Add(new Edge(thisNode,nextNode));
                        thisNode = nextNode;
                    }
                }
            }

            // place some uniformly distributed Nodes around the line. The longer the line,
            // the fewer places these Nodes can go, so we need to make sure we don't land
            // in an infinite loop trying to place them. This tries 1000 times.
            int lineCount = _initNodes.Count;
            int n = 0;
            while (_initNodes.Count - lineCount < numNodes && n < 1000)
            {
                Node node = new(rng.Between(0,63),rng.Between(0,63));
                if (!totalFootprint.Contains(node))
                {
                    _initNodes.Add(node);
                    totalFootprint.UnionWith(_footprint.LocusToLocus(node)); 
                }
                n++;
            }
            // Console.WriteLine($"Initialized {_initNodes.Count} Linear Random nodes in {n} loops.");
            return new Graph(_initNodes);    
        }
        //////////////////////////////////////////////////
        /// END Initial distribution methods
        
        /// This draws the first path between the _firstNode and its nearest neighbor.
        /// The output is the nearest neighbor, the Edge between them,
        /// the list of Nodes making up the drawn path, and the total footprint (with padding)
        /// those take up.
        private (Node, Edge, List<Node>, HashSet<Node>) FirstPath(MT19337 rng)
        {
            /// the most isolated Node is the one with the longest distance to its nearest neighbor.
            /// If we aren't drawing loops, we want the room to be at a leaf of the Graph, because
            /// there is only one path from the entrance to the room on a tree, and anything beyond
            /// the room is wasted space. The room and its first path nodes take up space, so making
            /// it isolated gives it room without clobbering other nodes.
            /// However, if we are allowing loops, we want the room to be allowed to live anywhere,
            /// including on a loop, which means we want to allow it to be close enough to other potential
            /// leaf nodes that it will allow a loop Edge to be drawn between them.
            if (_avoidLoops)
            {
                _firstNode = _pool.MostIsolatedNode(rng);
            }
            else
            {
                _firstNode = _pool.Nodes.PickRandom(rng);
            }
            _pathNodes.Add(_firstNode);
            /// get a queue of the _firstNode's adjacent Edges, ordered by weight.
            /// this could have been a PriorityQueue to do the sort in its minheap, but that felt fussy
            /// We need this because we need to make sure the nearest Node isn't accidentally in the
            /// room's footprint. If it is, it will be clobbered, and since we're basing the logic
            /// about whether to flip the room horizontally on the location of the nearest node, we
            /// need to find a new nearest neighbor.
            Queue<Edge> nearest = new(_pool.Adjacencies[_firstNode].OrderBy(i=>i.Weight));
            Node nearestNeighbor;
            Edge nearestEdge;
            int dX;
            int dY;
            while (true)
            {
                nearestEdge = nearest.Dequeue();
                nearestNeighbor = nearestEdge.NeighborOf(_firstNode);
                (dX,dY) = _firstNode.Delta(nearestNeighbor);

                // if the nearest node is to the left of the _firstNode as it is in vanilla,
                // we can use the vanilla room. The room will always directly north of the path element
                // leading to the door, and there will always be wall to the right of that path element.
                // this constraint makes the wall tile logic much, much easier, and also conforms
                // with the design aesthetic of having the room embedded in the rock wall.
                if (dX <= 0)
                {
                    _roomNode = _firstNode + (0,-5);
                    _flipRoom = false;
                }
                else
                // set the _fliproom flag, and draw the room above, and 4 tiles to the left, of the path
                // element leading to the door.
                {
                    _roomNode = _firstNode + (-4,-5);
                    _flipRoom = true;
                }
                /// make the _roomFootprint, which will be universally forbidden for path elements besides
                /// _firstNode
                _roomFootprint = _footprint.RoomLocusToLocus(_roomNode);
                if (!_roomFootprint.Contains(nearestNeighbor))
                {
                    break;
                }
            }

            Node thisNode = _firstNode;
            /// now, if the nearest neighbor to the _firstNode is above the room, the path drawing algorithm
            /// will have a very hard time reaching it through the room. We can help it out by drawing a path
            /// element to the side of the room that connects to the _firstNode's element.
            if (dY < 0)
            {
                Node next;
                if (_flipRoom)
                {
                    next = _firstNode + (4, rng.Between(1,2) * -1); 
                }
                else
                {
                    next = _firstNode + (-4, rng.Between(1,2) * -1);
                }
                _pathNodes.Add(next);
                thisNode = next;
            }
            // now draw the path
            (Node? resultNode, List<Node> branchNodes,HashSet<Node> branchFootprint) = 
                NextPath(thisNode,nearestNeighbor,_roomFootprint,rng);
            // if for some reason we couldn't reach the nearest neighbor, we need to bail and reroll.
            // we could delete the first Node and try again from here, but overall it's easier to reroll.
            // In that case we lose a tiny bit of time redrawing the initial distribution.
            if (resultNode != nearestNeighbor)
            {
                _insane = true;
            }
            return ((Node)resultNode, nearestEdge, branchNodes, branchFootprint);            
        }

        // The NextPath method calls the NextStep logic in a loop and tries to find a set of legal path elements between
        // the inputNode and the targetNode. If it can't, it returns null results and the caller in the Prim's algorithm
        // loop will reject this Edge.
        private (Node?, List<Node>, HashSet<Node>) NextPath(Node inputNode, Node targetNode, HashSet<Node> forbidden, MT19337 rng)
        {
            Node? thisNode = inputNode;
            List<Node> pathNodes = new();
            HashSet<Node> thisFootprint = new();
            int n = 0;
            // the step tolerance is converted to a value corresponding to the dot product between the direction
            // of the target node and the direction of potential GetNextStep() tiles.
            float stepTolerance = (float)Cos(_stepTolerance*PI/180.0);
            // build the path to the targetNode. If an iteration fails to find the next step given the current
            // stepTolerance and forbidden footprint, this tolerance is relaxed a bit at a time, all the way to
            // 90 degrees (or 0 in dot product terms). We get 120 total iterations to build the path.
            while (thisNode != targetNode && n < 120)
            {
                // try to get the next step.
                Node? nextNode = _nextStep.GetNextStep((Node)thisNode,targetNode,forbidden,stepTolerance,rng);
                n++;
                if (nextNode == null)
                {
                    stepTolerance = Max(stepTolerance - .01f,0);
                    continue;
                }
                pathNodes.Add((Node)nextNode);
                thisNode = nextNode;
                // reset the stepTolerance for the next step in case we have relaxed it to find this step
                stepTolerance = (float)Cos(_stepTolerance*PI/180.0);
            }
            // if we couldn't reach the targetNode in 120 iterations, return null values.
            if (thisNode != targetNode)
            {
                return (null,null,null);
            }
            
            // get the footprints of all the path nodes. We can use a smaller footprint if 
            // we don't care about loops -- this just occasionally lets paths erode holes into other paths
            foreach (Node node in pathNodes)
            {
                if (_avoidLoops)
                {
                    thisFootprint.UnionWith(_footprint.LocusToLocus(node));
                }
                else
                {
                    thisFootprint.UnionWith(_footprint.SmallLocusToLocus(node));
                }
            }
            return (targetNode,pathNodes,thisFootprint);
        }

        // this should probably be a static class, but I could foresee other procgen stuff with different
        // weighting strategies where you'd want multiple instances for different situations
        public class NextStep
        {
            /// this class maintains a set of 80 tiles about the center,* which form the legal potential
            /// steps on a connected path where the path element is 4x4 tiles. Four of these tiles
            /// actually produce a 2x1-tile wall, which is subsequently deleted in the tile-smoothing
            /// algorithm when the actual map is constructed. This kind of connection only happens
            /// a couple of times in the original waterfall map. It looks like this, where "o" is a path tile:
            /// 
            /// o o o o
            /// o o o o
            /// o o o o
            /// o o o o
            ///     o o      <- 2x1 connector
            ///     o o o o
            ///     o o o o
            ///     o o o o
            ///     o o o o
            /// 
            /// all the other next step tiles draw a path element that is either adjacent to or overlapping with
            /// the current Node's path element.
            /// 
            /// *actually, it's going to be 81 tiles total, but the weight of the current tile will always be 0
            
            /// the set of vectors to the surrounding NextStep tiles
            private readonly Vector2[] _nextStepVectors;

            /// unit vectors pointing at each of those tiles; this encodes
            /// the direction but not the magnitude, and does not scale magnitudes
            /// when used in dot products
            private readonly Vector2[] _nextStepUnitVectors;
            
            /// a set of weights based on distance from the center. The original waterfall map favors
            /// either adjacent map elements or elements with only a bit of overlap, so we weight potential
            /// steps correspondingly.
            private readonly float[] _weights;

            public NextStep()
            {
                // the legal next steps do not form a rectangle, so it's easier to construct it
                // in chunks and concatenate.
                Vector2[] next1 = {new(-2,-5),new(2,-5)};
                Vector2[] next2 = new Vector2[7];
                Vector2[] next3 = new Vector2[63];
                Vector2[] next4 = new Vector2[7];
                Vector2[] next5 = {new(-2,5),new(2,5)};
                for (int i = 0; i < 7; i++)
                {
                    next2[i] = new(i - 3, -4);
                    next4[i] = new(i - 3, 4);
                }
                for (int i = 0; i < 63; i++)
                {
                    int x = i % 9 - 4;
                    int y = i / 9 - 3;
                    next3[i] = new(x,y);
                }
                _nextStepVectors = next1.Concat(next2).Concat(next3).Concat(next4).Concat(next5).ToArray();

                // the unit vectors are just the vectors divided by length; if that length is 0, we get a 0/0 NaN, so
                // just substitute division by 1 there instead.
                _nextStepUnitVectors = _nextStepVectors.Select(v => v/(v.Length() == 0 ? 1 : v.Length())).ToArray();
 
                // this function makes weights that are very small at the center, increase to a peak where squared distance
                // from center is about 13, and then drop off very fast from there.
                // Scaling by 100 here lets us use these values in PickRandomItem(rng) without having to scale up to usable integers
                // every step
                _weights = _nextStepVectors
                                .Select(v => (float)(-100*Cos(2*PI*Pow(v.LengthSquared()/32.0,0.75)) + 100))
                                .ToArray();
                _weights[40] = 0; // zero out the center
            }

            public Node? GetNextStep(Node thisNode, Node targetNode, HashSet<Node> forbidden, float stepTolerance, MT19337 rng)
            {
                // get a unit vector pointing from here to the targetNode.
                Vector2 targetVector = thisNode.UnitVector(targetNode);
                
                // get tile weights for each surrounding next step tile
                float[] floatWeights = FloatWeights(targetVector, stepTolerance);
                
                // this will be the list used by PickRandomItemWeighted(rng)
                List<(Node node,int weight)> weightedSteps = new();

                
                // If the targetNode is within the nonzero-weighted next steps and is less than 5.0 in
                // distance, go ahead and return the targetNode to signal to the path drawing loop that the
                // path has found its target. We need the 5.0 limitation so that over time the paths do not favor
                // long steps at the end
                for (int i = 0; i < 81; i++)
                {
                    float thisWeight = floatWeights[i];
                    Node candidate = thisNode + _nextStepVectors[i];
                    if (candidate == targetNode && thisWeight != 0 && thisNode.SqDist(candidate) < 25)
                    {
                        return targetNode;
                    }
                    // otherwise, do some checks to filter out bad numbers that may have crept in via Vector2 length and dot product
                    // also filter out forbidden tiles. Add the remainder and their weight to the weightedSteps list.
                    if (!forbidden.Contains(candidate) && thisWeight > 0 && !float.IsNaN(thisWeight) && float.IsFinite(thisWeight))
                    {
                        // we use Ceiling here to catch any positive weighting no matter how small
                        weightedSteps.Add((candidate,(int)Ceiling(floatWeights[i])));
                    }
                }

                // sometimes PickRandomItemWeighted barfs if there is only one item, so in that case
                // just return the single node
                if (weightedSteps.Count > 1)
                {
                    return weightedSteps.PickRandomItemWeighted(rng);
                }
                else if (weightedSteps.Count == 1)
                {
                    return weightedSteps.Single().node;
                }
                // or if the list of legal moves is empty, return a null value.
                else
                {
                    return null;
                }                
            }

            private float[] FloatWeights(Vector2 target, float stepTolerance)
            {
                float[] dotProds = new float[81];
                float[] weights = new float[81];
                
                for (int i = 0; i < 81; i++)
                {
                    // dot products of the target Node's unit vector and those of the next step tiles.
                    // zero them out if they are less than the stepTolerance. This provides a weighting
                    // that allows tiles within the stepTolerance, and weights each one higher the closer
                    // it is to the direction of the target Node.
                    // 
                    float dP = Vector2.Dot(_nextStepUnitVectors[i],target);
                    dP = dP < stepTolerance ? 0 : dP;

                    // scale the distance weights by the dot-product weights.
                    weights[i] = _weights[i] * dP;
                }
                return weights;
            }
        }

        /// Strategies for placing the entrance.
        private Node GetEntranceNode(MT19337 rng)
        {
            Node outputNode = new();
            switch (_flags.ProcgenWaterfallEntrance)
            {
                // literally any node in the path nodes. 
                case ProcgenWaterfallEntrance.Anywhere:
                    {
                        outputNode = _pathNodes.PickRandom(rng);
                        break;
                    }
                // find the set of nodes in the _tree Graph with the most attached edges,
                // and return the furthest of these from the room
                case ProcgenWaterfallEntrance.Branch:
                    {
                        (var distances, var nodes) = AllDistances(_firstNode);
                        IEnumerable<Node> branchyNodes = _tree.Nodes.GroupBy(node => _tree.Adjacencies[node].Count).MaxBy(group => group.Key);
                        outputNode = branchyNodes.MaxBy(node => nodes[node]);
                        break;
                    }
                // find the most distant node from the first node; pick a random one of these in case of a tie.
                case ProcgenWaterfallEntrance.Furthest:
                    {
                        (var distances, var nodes) = AllDistances(_firstNode);
                        outputNode = distances[distances.Keys.Max()].PickRandom(rng);
                        break;
                    }
                // find a node that is within 40% to 60% of the longest distance from the first node.
                // if for some insane reason there aren't any, return the median instead.
                case ProcgenWaterfallEntrance.Mid:
                    {
                        (var distances, var nodes) = AllDistances(_firstNode);
                        int max = distances.Keys.Max();
                        List<int> midDists = distances.Keys.Where(d => d >= 0.4*max && d <= 0.6*max).ToList();
                        if (midDists.Count == 0)
                        {
                            midDists.Add(distances.Keys.OrderBy(d => d).ToList()[distances.Count/2]);
                        }
                        outputNode = distances[midDists.PickRandom(rng)].PickRandom(rng);
                        break;
                    }
                // find the centroid of the graph, i.e. the node with the minimum total distance from
                // all the other nodes. Pick a random one of these in case of tie.
                case ProcgenWaterfallEntrance.Center:
                    {
                        Dictionary<int,List<Node>> pathSums = new();
                        foreach (Node node in _tree.Nodes)
                        {
                            (var distances, var nodes) = AllDistances(node);
                            int sum = nodes.Values.Sum();
                            if (!pathSums.ContainsKey(sum))
                            {
                                pathSums[sum] = new();
                            }
                            pathSums[sum].Add(node);
                        }
                        outputNode = pathSums[pathSums.Keys.Min()].PickRandom(rng);
                        break;
                    }
                // try to spawn in visible distance of the room, but more than half the longest distance
                // to the room
                case ProcgenWaterfallEntrance.Maddening:
                    {
                        (var distances, var nodes) = AllDistances(_firstNode);
                        HashSet<Node> allNodes = _tree.Nodes.ToHashSet();
                        List<Node> nodesByWalkingDist = allNodes.OrderBy(n => nodes[n]).ToList();
                        List<Node> nodesByRectDist = allNodes.OrderByDescending(n => _firstNode.RectDist(n)).ToList();
                        List<Node> roomVisible = new();
                        foreach (Node node in allNodes)
                        {
                            /// find all the nodes within visible distance of the room. We know it's the room if
                            /// we can see a wall embedded in surrounding wall, or if we can see the door.
                            /// These numbers take into account that the stairway is placed at the Node coordinate
                            /// + 2 in both x and y
                            (int dX, int dY) = _roomNode.Delta(node);
                            if (_flipRoom)
                            {
                                if ((dX >= -10 && dX <= 10 && dY >= -9 && dY <= 8) || (dY == 9 && dX >= -5 && dX <= 10))
                                {
                                    roomVisible.Add(node);
                                }
                            }
                            else if ((dX >= -8 && dX <= 12 && dY>= -9 && dY <= 8) || (dY == 9 && dX >= -8 && dX <= 7))
                            {
                                roomVisible.Add(node);
                            }
                        }
                        // find any of them that are more than half the longest distance from the first node
                        roomVisible = roomVisible.Where(n => nodes[n] >= nodes[nodesByWalkingDist.Last()]/2).ToList();
                        // if there are any, return the furthest one
                        if (roomVisible.Count > 0)
                        {
                            outputNode = roomVisible.MaxBy(n => nodes[n]);
                        }
                        /// if no such nodes exist, then we take turns removing the closest walking-distance Node
                        /// and furthest RectDist Node to optimize the two criteria. Hopefully this means that we
                        /// can see the room in early exploration but not be on the immediate branch that reaches it.
                        else
                        {
                            while (allNodes.Count > 1)
                            {
                                allNodes.Remove(nodesByWalkingDist[0]);
                                nodesByWalkingDist.RemoveAt(0);
                                if (allNodes.Count == 1)
                                {
                                    break;
                                }
                                allNodes.Remove(nodesByRectDist[0]);
                                nodesByRectDist.RemoveAt(0);
                            }
                            outputNode = allNodes.Single();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return outputNode;
        }

        // Use Dijkstra's algorithm to get the walking distances of the inputNode to all other Nodes
        // through the graph edges. This seems like only an estimate because we're not including every pathNode,
        // only the control Nodes, but if two nodes are connected by an edge, the walking distance through
        // the path Nodes is going to be very, very close to the rectilinear distance (to within random veering of the path)
        // returns two dictionaries: the first uses distance as a key to look up corresponding Nodes, and the
        // second uses nodes as a key to return its distance. Both are useful in the entrance placement routine.
        private (Dictionary<int,List<Node>>,Dictionary<Node,int>)  AllDistances(Node inputNode)
        {
            Dictionary<int,List<Node>> distancesToNodes = new();
            Dictionary<Node,int> nodesToDistances = new();
            HashSet<Node> visitedNodes = new();
            
            /// start by setting the distance to each Node to ONE MILLION steps
            foreach (Node node in _tree.Nodes)
            {
                nodesToDistances[node] = 1000000;
            }
            /// but the inputNode's distance is 0 by definition.
            nodesToDistances[inputNode] = 0;
            visitedNodes.Add(inputNode);
            //this could be a priority queue, but there's no good reason for it to be with
            // such small Graphs.
            Queue<(Node,Edge)> edgeQueue = new();
            // enqueue all of the inputNode's edges
            foreach (Edge edge in _tree.Adjacencies[inputNode])
            {
                edgeQueue.Enqueue((inputNode,edge));
            }
            // main loop
            while (edgeQueue.Count != 0)
            {
                // dequeue an edge and get the neighbor
                (Node thisNode,Edge thisEdge) = edgeQueue.Dequeue();
                Node neighbor = thisEdge.NeighborOf(thisNode);
                // if we haven't visited the neighbor yet, mark it visited
                // and enqueue its edges.
                if (!visitedNodes.Contains(neighbor))
                {
                    visitedNodes.Add(neighbor);
                    foreach (Edge edge in _tree.Adjacencies[neighbor])
                    {
                        edgeQueue.Enqueue((neighbor,edge));
                    }
                }
                // we know the current shortest distance from _firstNode to this Node.
                int thisDist = nodesToDistances[thisNode];
                // we may or may not know the shortest distance to its neighbor; if we don't know it
                // it will be the default ONE MILLION steps.
                int neighborDist = nodesToDistances[neighbor];
                // add the rectilinear distance from thisNode to its neighber to the shortest known
                // distance to thisNode
                int candidateDist = thisDist + thisNode.RectDist(neighbor);
                // if the candidateDistance is lower than the current known distance from _firstNode to
                // thisNode's neighbor, then the best path to neighbor steps through thisNode and now
                // we have a new shortest path.
                if (candidateDist < neighborDist)
                {
                    nodesToDistances[neighbor] = candidateDist;
                    // we could maintain a list of predecessors in that path; if so, we would add it here.
                    // predecessors[neighbor] = (thisNode,candidateDist);
                }
            }

            // construct the distancesToNodes Dictionary
            foreach (Node node in visitedNodes)
            {
                int dist = nodesToDistances[node];
                if (!distancesToNodes.ContainsKey(dist))
                {
                    distancesToNodes[dist] = new();
                }
                distancesToNodes[dist].Add(node);
            }
            return (distancesToNodes,nodesToDistances);
        }
        
        
        // this class maintains a set of "footprints," i.e. collection of nodes corresponding
        // to patterns in waterfall design. 
        public class Footprint
        {            

            // these fields are accessible only by accessor methods below. Some of them
            // are never actually used in the algorithm, but are intermediate steps in constructing
            // more complex footprints. The shapes are formed via convolution, i.e.
            // for each point in set A, give me an entire copy of set B centered on that point
            // and return the union
            // each shape is drawn with the following tiles:
            // o   the (0,0) origin
            // x   a path tile
            // w   a wall tile
            // p   padding tiles

            // the 4x4 path element
            // o x x x
            // x x x x
            // x x x x
            // x x x x
            private readonly HashSet<Node> _path;


            // a path element with the smallest possible surrounding walls (any smaller will generate tile discontinuities)
            //     w w w w
            //     w w w w
            //     w w w w
            // w w o x x x w w
            // w w x x x x w w
            // w w x x x x w w
            // w w x x x x w w
            //     w w w w
            //     w w w w
            //     w w w w
            private readonly HashSet<Node> _boundary;


            // This is the most important one. It includes the padding such that any path element drawn at a coordinate
            // inside the padding will either connect the paths or produce a tile discontinuity. This is used for keeping
            // separate branches of the overall path from colliding in bad ways.

            //     p p p p p p p
            //     p p p p p p p
            //     p p p p p p p
            // p p p p p w w w w p p
            // p p p p p w w w w p p
            // p p p p p w w w w p p
            // p p p w w o x x x w w
            // p p p w w x x x x w w
            // p p p w w x x x x w w
            // p p p w w x x x x w w
            //     p p p w w w w
            //     p p p w w w w
            //     p p p w w w w
            private readonly HashSet<Node> _locusToLocus;


            // The room itself
            
            // o w w w w w w w
            // w x x x x x x w
            // w x x x x x x w
            // w x x x x x x w
            // w w w w w w w w
            private readonly HashSet<Node> _room;


            // padding around the room giving forbidden nodes

            // p p p p p p p p p p p
            // p p p p p p p p p p p
            // p p p p p p p p p p p
            // p p p o w w w w w w w
            // p p p w x x x x x x w
            // p p p w x x x x x x w
            // p p p w x x x x x x w
            // p p p w w w w w w w w
            // p p p p p p p p p p p
            // p p p p p p p p p p p
            // p p p p p p p p p p p
            private readonly HashSet<Node> _roomLocusToLocus;


            // relaxed Locus-to-Locus footprint used when we don't care about loops

            // p p p p p p p
            // p p p p p p p
            // p p p p p p p
            // p p p o x x x
            // p p p x x x x
            // p p p x x x x
            // p p p x x x x
            private readonly HashSet<Node> _smallLocusToLocus;


            // never used as an actual footprint, but kept around just in case
            // this is the shape of the tiles surrounding the center in NextStep
            
            //     x       x
            //   x x x x x x x 
            // x x x x x x x x x
            // x x x x x x x x x
            // x x x x x x x x x
            // x x x x o x x x x
            // x x x x x x x x x
            // x x x x x x x x x
            // x x x x x x x x x
            //   x x x x x x x
            //     x       x
            private readonly HashSet<Node> _nextStepPool;

            private readonly (int,int)[] _pathKernel =
            {
                (0,0),(1,0),(2,0),(3,0),
                (0,1),(1,1),(2,1),(3,1),
                (0,2),(1,2),(2,2),(3,2),
                (0,3),(1,3),(2,3),(3,3)
            };

            private readonly (int,int)[] _boundaryKernel = 
            {
                       (0,-3),
                (-2,0),       ( 2,0),
                       (0, 3)
            };

            private readonly (int,int)[] _locusToLocusKernel =
            {
                (-3,-3),( 0,-3),
                (-3, 0),( 0, 0)
            };


            private readonly (int,int)[] _roomKernel =
            {
                (0,0),(4,0),
                (0,1),(4,1)
            };

            private readonly (int,int)[] _roomLocusToLocusKernel =
            {
                (-3,-3),( 0,-3),
                (-3, 0),( 0, 0),
                (-3, 3),( 0, 3)  
            };

            private readonly (int,int)[] _nextStepPoolKernel =
            {
                        (-2,-3),( 1,-3),
                (-3,-2),                ( 2,-2),
                (-3, 1),                ( 2,-1),
                        (-2, 2),( 1,-2)
            };

            public Footprint()
            {
                InitFootprint(new HashSet<Node> {new(0,0)}, _pathKernel, out _path);
                InitFootprint(_path,_boundaryKernel,out _boundary);
                InitFootprint(_boundary,_locusToLocusKernel,out _locusToLocus);
                InitFootprint(_path,_roomKernel,out _room);
                InitFootprint(_room,_roomLocusToLocusKernel,out _roomLocusToLocus);
                InitFootprint(_path,_nextStepPoolKernel,out _nextStepPool);
                    _nextStepPool.UnionWith(new Node[] {new(-2,-5),new(2,-5),new(-2,5),new(2,5)});
                InitFootprint(_path,_locusToLocusKernel,out _smallLocusToLocus);
            }


            /// convolution of footprint kernels.
            private void InitFootprint(HashSet<Node> sourceFootprint, (int,int)[] kernel, out HashSet<Node> outFootprint)
            {
                outFootprint = new();
                foreach ((int,int) p in kernel)
                {
                    outFootprint.UnionWith(CenteredFootprint(new Node(p),sourceFootprint));
                }
            }

            // return a footprint centered on the input Node
            private HashSet<Node> CenteredFootprint(Node node,HashSet<Node> footprint)
            {
                HashSet<Node> newFootprint = new();
                foreach (Node n in footprint)
                {
                    newFootprint.Add(node + n);
                }
                return newFootprint;
            }
            public HashSet<Node> Path(Node node)
            {
                return CenteredFootprint(node,_path);
            }

            public HashSet<Node> Boundary(Node node)
            {
                return CenteredFootprint(node,_boundary);
            }

            public HashSet<Node> LocusToLocus(Node node)
            {
                return CenteredFootprint(node,_locusToLocus);
            }

            public HashSet<Node> Room(Node node)
            {
                return CenteredFootprint(node,_room);
            }

            public HashSet<Node> RoomLocusToLocus(Node node)
            {
                return CenteredFootprint(node,_roomLocusToLocus);
            }

            public HashSet<Node> NextStepPool(Node node)
            {
                return CenteredFootprint(node,_nextStepPool);
            }

            public HashSet<Node> SmallLocusToLocus(Node node)
            {
                return CenteredFootprint(node,_smallLocusToLocus);
            }
        }
    }
}