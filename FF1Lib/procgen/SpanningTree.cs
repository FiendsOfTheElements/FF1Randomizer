using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.Processing.Processors.Convolution;
using static System.Math;


namespace FF1Lib.Procgen
{
    public class SpanningTree : IMapGeneratorEngine
    {
        /// uses Prim's algorithm to create a minimum spanning tree over a set of randomly chosen nodes;
        /// This is really only ever going to be used for waterfall

        private Graph _pool;
        //private Graph _tree;

        private Node _firstNode;

        private Node _roomNode;
        //private Node _entranceNode;

        private Footprint _footprint;

        private bool _flipRoom = false;

        private List<Node> _initNodes;

        

        
        public SpanningTree()
        {
            _footprint = new();
            _initNodes = new();
        }



        public CompleteMap Generate(MT19337 rng, MapRequirements reqs)
        {
            PriorityQueue<Edge,int> edgeQueue = new();
            Node nearest = InitPoolGraph(rng);

            HashSet<Node> roomFootprint = _footprint.Room(_roomNode);
            // HashSet<Node> totalFootprint =

            CompleteMap cm = new()
            {
                Map = new((byte)Tile.WaterfallRandomEncounters),
                Requirements = reqs
            };

            var room = reqs.Rooms.Single();
            var robot = room.NPCs.Single();

            cm.Map.Put(_roomNode.XY,room.Tiledata);
            if (_flipRoom)
            {
                robot.Coord.x = room.Width - robot.Coord.x;
                cm.Map.FlipSectionHorizontal(_roomNode.X + 1,_roomNode.Y + 1,_roomNode.X+room.Width - 2, _roomNode.Y+room.Height - 1);
            }

            robot.Coord.x += _roomNode.X;
            robot.Coord.y += _roomNode.Y;
            reqs.MapObjects.SetNpc(robot.Index, robot.ObjectId, robot.Coord.x, robot.Coord.y, robot.InRoom, robot.Stationary);

            


            



            //////////////////////////
            /// //////////////////////
            /// //////////////////////
            foreach (Node n in _pool.Nodes)
            {
                byte tile = n == nearest? (byte)Tile.Ladder : (byte)0xFE;
                cm.Map.Put(n.XY,new byte[,] {{tile}});
            }
            // foreach (Node n in _pool.Nodes)
            // {
            //     cm.Map.Put(n.XY,new byte[,] {{(byte)0xFE}});
            // }
            // foreach (Node n in roomFootprint)
            // {
            //     // byte tile = n == _firstNode? (byte)Tile.Door : (byte)Tile.LadderHole;
            //     cm.Map.Put(n.XY,new byte[,] {{(byte)Tile.LadderHole}});
            // }
            cm.Map.Put(_firstNode.XY,new byte[,] {{(byte)Tile.LadderHole}});
            if (_flipRoom)
            {
                Console.WriteLine("Flip Room");
            }

            Console.Write(cm.AsText());
            return cm;
        }
        
        private Node InitPoolGraph(MT19337 rng)
        {
            Func<MT19337,Graph> InitDistribution = new List<Func<MT19337,Graph>>() {UniformRandom, PolarRandom, JitteredEvenRandom, LinearRandom}.PickRandom(rng);
            _pool = InitDistribution(rng);

            // _pool = LinearRandom(rng);

            _firstNode = _pool.MostIsolatedNode(rng);
            Queue<Edge> nearest = new(_pool.Adjacencies[_firstNode].OrderBy(i=>i.Weight));
            Node nearestNeighbor;
            HashSet<Node> roomFootprint;
            while (true)
            {

                nearestNeighbor = nearest.Dequeue().NeighborOf(_firstNode);
                int dX = _firstNode.Delta(nearestNeighbor).dX;
                if (dX <= 0)
                {
                    _roomNode = _firstNode + (0,-5);
                    _flipRoom = false;
                }
                else
                {
                    _roomNode = _firstNode + (-4,-5);
                    _flipRoom = true;
                }
                roomFootprint = _footprint.RoomLocusToLocus(_roomNode).Union(_footprint.LocusToLocus(_firstNode)).ToHashSet();
                if (!roomFootprint.Contains(nearestNeighbor))
                {
                    break;
                }
            }
            _pool.Remove(roomFootprint.Intersect(_pool.Nodes));
            return nearestNeighbor;
        }

        // Get a Graph of nodes chosen randomly from the 64x64, spaced so that a full wall can be
        // drawn around each node. We hope the distribution ends up a little clumpy
        private Graph UniformRandom(MT19337 rng)
        {

            
            int numNodes = rng.Between(27,37);
            HashSet<Node> totalFootprint = new();
            int n = 0;
            while (_initNodes.Count < numNodes)
            {
                Node node = new(rng.Between(0,63),rng.Between(0,63));
                if (!totalFootprint.Contains(node))
                {
                    _initNodes.Add(node);
                    totalFootprint.UnionWith(_footprint.LocusToLocus(node));
                    n++;
                }
            }
            Console.WriteLine($"Initialized {_initNodes.Count} Uniform Random nodes in {n} loops.");
            return new Graph(_initNodes);
        }

        // Get a graph of nodes chosen a random angle and distance from the center in polar coordinates.
        // This should tend to distribute them more densely around the center.
        private Graph PolarRandom(MT19337 rng)
        {
            //HashSet<Node> pool = GetAllNodes();
            int numNodes = rng.Between(27,37);
            HashSet<Node> totalFootprint = new();

            int n = 0;
            const double degToRad = PI/180.0;
            int width = 1200;

            while (_initNodes.Count < numNodes )
            {
                /// get random degrees
                int theta = rng.Between(0,359);
                /// get random distance. (theta + 45) % 90 - 45 normalizes the angle to the range
                /// -45 to +45, which lets us get the distance from the center to the edge of the
                /// square perimeter.
                int dist = rng.Between(0,(int)Round(width/Cos(((theta + 45)%90 - 45) * degToRad)));
                // convert polar to rectangular
                int X = (int)Round((3200.0  + dist*Cos(theta * degToRad))/100.0);
                int Y = (int)Round((3200.0 + dist*Sin(theta * degToRad))/100.0);
                Node node = new(X,Y);
                if (!totalFootprint.Contains(node))
                {
                    _initNodes.Add(node);
                    totalFootprint.UnionWith(_footprint.LocusToLocus(node));
                }
                else
                {
                    width = Min(width + 50,3200);
                }
                n++;
            }
            Console.WriteLine($"Initialized {_initNodes.Count} Polar Random nodes in {n} loops.");       
            return new Graph(_initNodes);
        }

        // this produces an almost even distribution of nodes. 
        private Graph JitteredEvenRandom(MT19337 rng)
        {

            //list of points around the center
            List<(int,int)> kernel = new()
            {
                (-2,-2),(-1,-2),( 0,-2),( 1,-2),( 2,-2),
                (-2,-1),(-1,-1),( 0,-1),( 1,-1),( 2,-1),
                (-2, 0),(-1, 0),( 0, 0),( 1, 0),( 2, 0),
                (-2, 1),(-1, 1),( 0, 1),( 1, 1),( 2, 1),
                (-2, 2),(-1, 2),( 0, 2),( 1, 2),( 2, 2)  
            };
            // weights associated with a normal distribution about the center,
            // by distance (trust me bro)
            List<int> normalWeights = new()
            {
                 185, 148, 346, 148, 185,
                 148, 473, 553, 473, 148,
                 346, 553,2586, 553, 346,
                 148, 473, 553, 473, 148,
                 185, 148, 346, 148, 185
            };
            List<int> uniformWeights = new()
            {
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1
            };
            List<int> weights = new List<List<int>>() {normalWeights, uniformWeights}.PickRandom(rng);
            for (int y = 0; y < 8; y++ )
            {
                for (int x = 0; x < 4; x++)
                {
                    /// the distribution before jitter looks like this
                    /// .   .   .   .
                    ///   .   .   .   .
                    /// .   .   .   .
                    ///   .   .   .   .
                    /// .   .   .   .
                    ///   .   .   .   .
                    /// .   .   .   .
                    ///   .   .   .   .
                    
                    /// making 32 nodes total
                    Node center = new(x*16 + y % 2 * 8, y*8);
                    /// for each center, get a list of points about that center and select one based on the weights above. Clumps
                    /// around the center
                    _initNodes.Add(kernel.Select(i => center + i).Zip(weights).ToList().PickRandomItemWeighted(rng));
                }
            }
            Console.WriteLine($"Initialized {_initNodes.Count} Jittered Random nodes.");
            return new Graph(_initNodes);
        }

        private Graph LinearRandom(MT19337 rng)
        {

            HashSet<Node> totalFootprint = new();
            int numNodes = rng.Between(22,32);
            int theta = rng.Between(0,179);
            double slope = 0.0;
            double degToRad = PI/180.0;
            List<Node> line = new();
            Console.WriteLine($"Theta: {theta}");


            if (theta <= 45 || theta >= 135 )
            {
                slope = Tan(theta * degToRad);
                Console.WriteLine($"Slope: {slope}");
                double y = 32.0 * slope + 32.0;
                int limit = rng.Between(64,192);
                for (int x = 0; x < limit; x++, y-=slope)
                {
                    Node node = new(x,(int)Round(y));
                    if (!totalFootprint.Contains(node))
                    {
                        _initNodes.Add(node);
                        totalFootprint.UnionWith(_footprint.LocusToLocus(node));
                    }
                }
            }
            else
            {
                slope = theta == 90? 0 : 1.0/Tan(theta * degToRad);
                double x = 32.0 * slope + 32.0;
                Console.WriteLine($"Slope: {slope}");
                for (int y = 0; y < 64; y++, x-=slope)
                {
                    Node node = new((int)Round(x),y);
                    if (!totalFootprint.Contains(node))
                    {
                        _initNodes.Add(node);
                        totalFootprint.UnionWith(_footprint.LocusToLocus(node));
                    }
                }
            }

            int lineCount = _initNodes.Count;
            int n = 0;
            while (_initNodes.Count - lineCount < numNodes)
            {
                Node node = new(rng.Between(0,63),rng.Between(0,63));
                if (!totalFootprint.Contains(node))
                {
                    _initNodes.Add(node);
                    totalFootprint.UnionWith(_footprint.LocusToLocus(node));
                    n++;
                }
            }
            Console.WriteLine($"Initialized {_initNodes.Count} Linear Random nodes in {n} loops.");
            return new Graph(_initNodes);    
        }

        private HashSet<Node> GetAllNodes()
        {
            HashSet<Node> nodes = new();
            for (byte x = 0; x < 64; x++)
            {
                for (byte y = 0; y < 64; y++)
                {
                    nodes.Add(new Node(x,y));
                }
            }
            return nodes;
        }
        

        // private (Node pathInit,bool flipRoom) RoomPath(MT19337 rng)
        // {
        //     Edge nearest = _pool.Adjacencies[_firstNode].OrderBy(i=>i.Weight).First();
        //     Node nearestNeighbor = nearest.NeighborOf(_firstNode);
        //     int dX = _firstNode.Delta(nearestNeighbor).dX;
        //     Node pathNode;
        //     Node leftPath = _firstNode + (-2,4);
        //     Node rightPath = _firstNode + (2,4);
        //     if (dX < 0)
        //     {
        //         pathNode = leftPath;
        //     }
        //     else if (dX > 0)
        //     {
        //         pathNode = rightPath;
        //     }
        //     else
        //     {
        //         pathNode = new List<Node>() {leftPath,rightPath}.PickRandom(rng);
        //     }

        //     return (pathNode,pathNode == rightPath);
            
        // }

        private double DotProduct ((double X, double Y) left, (double X, double Y) right)
        {
            return left.X*right.X + left.Y*right.Y;
        }

        

        public class Footprint
        {
            // tile patterns associated with waterfall design

            // waterfall walkable path consists of overlapping 4x4 squares of tiles
            // 
            // public static readonly (int,int)[] Path =

            private readonly HashSet<Node> _path;
            private readonly HashSet<Node> _boundary;
            private readonly HashSet<Node> _centerToCenter;
            private readonly HashSet<Node> _room;
            private readonly HashSet<Node> _roomLocusToLocus;
            private readonly HashSet<Node> _nextStepPool;

            
            // private readonly (int,int)[] _pathKernel = 
            // {
            //     (-1,-1),( 0,-1),( 1,-1),( 2,-1),
            //     (-1, 0),( 0, 0),( 1, 0),( 2, 0),
            //     (-1, 1),( 0, 1),( 1, 1),( 2, 1),
            //     (-1, 2),( 0, 2),( 1, 2),( 2, 2)   
            // };

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

            // private readonly (int,int)[] _centerToCenterKernel =
            // {
            //     (-2,-2),( 1,-2),
            //     (-2, 1),( 1, 1)
            // };

            private readonly (int,int)[] _centerToCenterKernel =
            {
                (-3,-3),( 0,-3),
                (-3, 0),( 0, 0)
            };

            // private readonly (int,int)[] _roomKernel =
            // {
            //     (-2,-1),( 2,-1),
            //     (-2, 0),( 2, 0)
            // };

            private readonly (int,int)[] _roomKernel =
            {
                (0,0),(4,0),
                (0,1),(4,1)
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
                InitFootprint(_boundary,_centerToCenterKernel,out _centerToCenter);
                InitFootprint(_path,_roomKernel,out _room);
                InitFootprint(_room,_centerToCenterKernel,out _roomLocusToLocus);
                InitFootprint(_path,_nextStepPoolKernel,out _nextStepPool);
                    _nextStepPool.UnionWith(new Node[] {new(-2,-5),new(2,-5),new(-2,5),new(2,5)});
            }


            private void InitFootprint(HashSet<Node> sourceFootprint, (int,int)[] kernel, out HashSet<Node> outFootprint)
            {
                outFootprint = new();
                foreach ((int,int) p in kernel)
                {
                    outFootprint.UnionWith(CenteredFootprint(new Node(p),sourceFootprint));
                }
            }


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
                return CenteredFootprint(node,_centerToCenter);
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
        }




        public class Graph
        {
            public readonly HashSet<Node> Nodes;
            public readonly HashSet<Edge> Edges;
            public readonly Dictionary<Node,HashSet<Edge>> Adjacencies;

            public Graph()
            {
                Nodes = new();
                Edges = new();
                Adjacencies = new();
            }

            public Graph(IEnumerable<Edge> edges) : this()
            {
                
                foreach (Edge e in edges)
                {
                    Add(e);
                }
            }


            public Graph(IEnumerable<Node> nodes) : this()
            {
                // get rid of duplicates
                List<Node> listNodes = nodes.ToHashSet().ToList();
                for (int i = 0; i < listNodes.Count - 1; i++)
                {
                    for (int j = i+1; j < listNodes.Count; j++)
                    {
                        var node1 = listNodes[i];
                        var node2 = listNodes[j];
                        Add(new Edge(node1,node2));
                    }
                }
            }

            public Node MostIsolatedNode(MT19337 rng)
            {
                return Nodes.GroupBy(i => Adjacencies[i].Sum(j => j.Weight))
                    .OrderBy(k => k.Key).Last().ToList().PickRandom(rng);
            }

            public void Add(Edge edge)
            {
                if (Edges.Contains(edge))
                {
                    return;
                }
                Node node1 = edge.Node1;
                Node node2 = edge.Node2;
                Edges.Add(edge);
                Nodes.Add(node1);
                Nodes.Add(node2);
                if (!Adjacencies.ContainsKey(node1))
                {
                    Adjacencies[node1] = new();
                }
                if (!Adjacencies.ContainsKey(node2))
                {
                    Adjacencies[node2] = new();
                }
                Adjacencies[node1].Add(edge);
                Adjacencies[node2].Add(edge);
            }

            public void Add(Node node)
            {
                //to add a node, add an edge from this node to every other node in the graph
                if (Nodes.Contains(node))
                {
                    return;
                }
                foreach (Node n in Nodes)
                {
                    Add(new Edge(n,node));
                }
            }

            public bool Remove(Edge edge)
            {
                if (!Edges.Contains(edge))
                {
                    return false;
                }
                Node node1 = edge.Node1;
                Node node2 = edge.Node2;
                Adjacencies[node1].Remove(edge);
                Adjacencies[node2].Remove(edge);
                if (Adjacencies[node1].Count == 0)
                {
                    Adjacencies.Remove(node1);
                    Nodes.Remove(node1);
                }
                if (Adjacencies[node2].Count == 0)
                {
                    Adjacencies.Remove(node2);
                    Nodes.Remove(node2);
                }
                return Edges.Remove(edge);
            }

            public bool Remove(IEnumerable<Edge> edges)
            {
                bool found = true;
                foreach (Edge e in edges)
                {
                    found = found && Remove(e);
                }
                return found;
            }

            public bool Remove(Node node)
            {
                if (!Nodes.Contains(node))
                {
                    return false;
                }
                else
                {
                    foreach (Edge e in Adjacencies[node].ToList())
                    {
                        Remove(e);
                    }
                    return true;
                }
            }

            public bool Remove(IEnumerable<Node> nodes)
            {
                bool found = true;
                foreach (Node n in nodes)
                {
                    found = found && Remove(n);
                }
                return found;
            }

            

            public bool Contains(Edge edge)
            {
                return Edges.Contains(edge);
            }

            public bool Contains(Node node)
            {
                return Nodes.Contains(node);
            }
            
        }

 

        
        // Node has value semantics.
		public readonly struct Node
		{
            private readonly byte _x = 0;
            private readonly byte _y = 0;
            public byte X
            {
                get => _x;
                
                init
                {
                    _x = value;
                    _x &= 0x3F;
                }
            }
            public byte Y
            {
                get => _y;

                init
                {
                    _y = value;
                    _y &= 0x3F;
                }
            }

            public (byte,byte) XY
            {
                get => (_x,_y);

                init
                {
                    X = value.Item1;
                    Y = value.Item2;
                }
            }

            public Node(byte x, byte y)
            {
                XY = (x,y);
            }

            public Node((byte,byte) coord)
            {
                XY = coord;
            }

            public Node(int x, int y)
            {
                XY = ((byte)x,(byte)y);
            }

            public Node((int,int) coord)
            {
                XY = ((byte,byte))coord;
            }

			public override string ToString()
			{
				return $"({X}, {Y})";
			}

            public static Node operator +(Node left, Node right)
            {
                return new Node((byte)(left.X + right.X),(byte)(left.Y + right.Y));
            }

            public static Node operator +(Node left, (byte,byte) right)
            {
                return new Node((byte)(left.X + right.Item1),(byte)(left.Y + right.Item2));
            }

            public static Node operator +(Node left, (int,int) right)
            {
                return new Node((byte)(left.X + right.Item1),(byte)(left.Y + right.Item2));
            }

            public static Node operator -(Node left, Node right)
            {
                return new Node((byte)(left.X - right.X),(byte)(left.Y - right.Y));
            }

            public static Node operator -(Node left, (byte,byte) right)
            {
                return new Node((byte)(left.X - right.Item1),(byte)(left.Y - right.Item2));  
            }

            public static Node operator -(Node left, (int,int) right)
            {
                return new Node((byte)(left.X - right.Item1),(byte)(left.Y - right.Item2));
            }

			public override bool Equals([NotNullWhen(true)] object obj)
			{
				return base.Equals(obj);
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

            public static bool operator ==(Node left, Node right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Node left, Node right)
            {
                return !left.Equals(right);
            }

            public static bool operator >(Node left, Node right)
            {
                if (left.X != right.X)
                {
                    return left.X > right.X;
                }
                else
                {
                    return left.Y > right.Y;
                }
                
            }
            public static bool operator <(Node left,Node right)
            {
                if (left.X != right.X)
                {
                    return left.X < right.X;
                }
                else
                {
                    return left.Y < right.Y;
                } 
            }

            public int SqDist(Node other)
            {
                int dx = (X - other.X + 64) % 64;
                dx = Min(dx,64-dx);
                
                int dy = (Y - other.Y + 64) % 64;
                dy = Min(dy,64-dy);
                
                return dx*dx + dy*dy;
            }

            public double Dist(Node other)
            {
                return Sqrt(SqDist(other));
            }

            
            public (int dX,int dY) Delta(Node other)
            {
                int dx1 = other.X - X;
                int dx2 = 64-dx1;
                int dy1 = other.Y - Y;
                int dy2 = 64-dy1;
                int dx = Abs(dx1) <= Abs(dx2) ? dx1 : dx2;
                int dy = Abs(dy1) <= Abs(dy2) ? dy1 : dy2 ;
                

                return (dx,dy);
            }

            public (double X, double Y) UnitVector(Node other)
            {
                double dist = Dist(other);
                var (dX, dY) = Delta(other);
                return (dX/dist, dY/dist);
            }
        }

        public readonly struct Edge
        {
            private readonly (Node,Node) _nodes = (new(),new());
            private readonly int _weight = 0;

            public (Node,Node) Nodes
            {
                get => _nodes;
                init
                {
                    _nodes = value;        
                }
            }

            public Node Node1
            {
                get => _nodes.Item1;
            }

            public Node Node2
            {
                get => _nodes.Item2;
            }

            public int Weight
            {
                get => _weight;
                init
                {
                    _weight = value;
                }
            }

            // 
            

            public Edge(Node node1,Node node2)
            {
                if (node1!=node2)
                {
                    Nodes = node2 > node1 ? (node1,node2) : (node2,node1);
                    Weight = node1.SqDist(node2);
                }
                else
                {
                    throw new ArgumentException("Error: Nodes in Edges must have different coordinates.");
                }
            }

            public Node NeighborOf(Node node)
            {
                if (node == Node1)
                {
                    return Node2;
                }
                else if (node == Node2)
                {
                    return Node1;
                }
                else
                {
                    throw new ArgumentException($"Error: requested neighbor of Node {node} from an Edge that does not contain it.");
                }
            }

			public readonly override bool Equals([NotNullWhen(true)] object obj)
			{
				return base.Equals(obj);
			}

			public readonly override int GetHashCode()
			{
				return base.GetHashCode();
			}

            public static bool operator ==(Edge left, Edge right)
            {
                return left.Nodes == right.Nodes;
            }

            public static bool operator !=(Edge left, Edge right)
            {
                return !(left == right);
            }
        }
            
        
       



    }


}