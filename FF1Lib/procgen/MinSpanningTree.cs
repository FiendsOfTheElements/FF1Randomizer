using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing.Processors.Convolution;
using static System.Math;


namespace FF1Lib.Procgen
{
    public class MinSpanningTree : IMapGeneratorEngine
    {
        /// uses Prim's algorithm to create a minimum spanning tree over a set of randomly chosen nodes;
        /// 

        private Graph _pool;
        private Graph _tree;

        private Node _roomNode;
        private Node _stairsNode;

        private Footprint _footprint;

        
        public MinSpanningTree()
        {
            _footprint = new();
        }


        public CompleteMap Generate(MT19337 rng, MapRequirements reqs)
        {
            PriorityQueue<Edge,int> edgeQueue = new();
            int maxNodes = rng.Between(25,45);
            InitPoolGraph(maxNodes,rng);
            
            return new CompleteMap();
        }
        
        private void InitPoolGraph(int limit, MT19337 rng)
        {
            HashSet<Node> source = new();
            List<Node> nodes = new();
            for (byte x = 0; x < 64; x++)
            {
                for (byte y = 0; y < 64; y++)
                {
                    source.Add(new Node(x,y));
                }
            }

            while (source.Count > 0 && nodes.Count < limit)
            {
                Node center = source.PickRandom(rng);
                nodes.Add(center);
                source.ExceptWith(_footprint.CenterToCenter(center));
            }
            Console.WriteLine($"Initialized {nodes.Count} points.");
            
            _pool = new Graph(nodes);

            _roomNode = _pool.MostIsolatedNode(rng);
            foreach (Node n in _footprint.RoomCenterToCenter(_roomNode).Intersect(_pool.Nodes))
            {
                _pool.RemoveNode(n);
            }            
        }

        private void AddRoomBranch()
        {
            Edge nearest = _pool.Adjacencies[_roomNode].OrderBy(i=>i.Weight).First();
            Node nearestNeighbor = nearest.NeighborOf(_roomNode);
            (int dx, int dy) = _roomNode.Delta(nearestNeighbor);
            
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
                        Add(new(node1,node2));
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

            public bool RemoveNode(Node node)
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

            

            public bool Contains(Edge edge)
            {
                return Edges.Contains(edge);
            }

            public bool Contains(Node node)
            {
                return Nodes.Contains(node);
            }
            
        }

 

        
        // Node has value semantics. We'll build the graph itself with GraphNode.
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
                //probably don't need this but here to be complete
                return Sqrt(SqDist(other));
            }

            
            public (int,int) Delta(Node other)
            {
                // reciprocal of slope -- just keeps x,y in normal order, even though "slope" is "rise over run"
                int dx = Abs(X - other.X) <= Abs(other.X - X) ? X - other.X : other.X - X;
                int dy = Abs(Y - other.Y) <= Abs(other.Y - Y) ? Y - other.Y : other.Y - Y;

                return (dx,dy);
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

        public class Footprint
        {
            // tile patterns associated with waterfall design

            // waterfall walkable path consists of overlapping 4x4 squares of tiles
            // 
            // public static readonly (int,int)[] Path =


            

            private HashSet<Node> _path;
            private HashSet<Node> _boundary;
            private HashSet<Node> _centerToCenter;
            private HashSet<Node> _room;
            private HashSet<Node> _roomBoundary;
            private HashSet<Node> _roomCenterToCenter;

            
            private readonly (int,int)[] _pathKernel = 
            {
                (-1,-1),( 0,-1),( 1,-1),( 2,-1),
                (-1, 0),( 0, 0),( 1, 0),( 2, 0),
                (-1, 1),( 0, 1),( 1, 1),( 2, 1),
                (-1, 2),( 0, 2),( 1, 2),( 2, 2)   
            };

            private readonly (int,int)[] _boundaryKernel = 
            {
                        (0,-3),
                (-2,0),       (2,0),
                        (0,-2)
            };

            private readonly (int,int)[] _centerToCenterKernel =
            {
                (-2,-2),( 1,-2),
                (-2, 1),( 1, 1)
            };

            private readonly (int,int)[] _roomKernel =
            {
                (-2,-1),( 2,-1),
                (-2, 0),( 2, 0)
            };
           


            

            public Footprint()
            {
                InitFootprint(new HashSet<Node> {new(0,0)}, _pathKernel, out _path);
                InitFootprint(_path,_boundaryKernel,out _boundary);
                InitFootprint(_boundary,_centerToCenterKernel,out _centerToCenter);
                InitFootprint(_path,_roomKernel,out _room);
                InitFootprint(_room,_boundaryKernel,out _roomBoundary);
                InitFootprint(_roomBoundary,_centerToCenterKernel,out _roomCenterToCenter);
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

            public HashSet<Node> CenterToCenter(Node node)
            {
                return CenteredFootprint(node,_centerToCenter);
            }

            public HashSet<Node> Room(Node node)
            {
                return CenteredFootprint(node,_room);
            }

            public HashSet<Node> RoomBoundary(Node node)
            {
                return CenteredFootprint(node,_roomBoundary);
            }

            public HashSet<Node> RoomCenterToCenter(Node node)
            {
                return CenteredFootprint(node,_roomCenterToCenter);
            }

            // this tile pattern includes the minimum set of walls that could surround one 4x4 path square,
            // such that if a path from another branch encroached on these tiles, the walls could not be
            // drawn correctly
            // public static readonly (int,int)[] Boundary =
            // {
            //                     (-1,-4),( 0,-4),( 1,-4),( 2,-4),
            //                     (-1,-3),( 0,-3),( 1,-3),( 2,-3),
            //                     (-1,-2),( 0,-2),( 1,-2),( 2,-2),
            //     (-3,-1),(-2,-1),(-1,-1),( 0,-1),( 1,-1),( 2,-1),( 3,-1),( 4,-1),
            //     (-3, 0),(-2, 0),(-1, 0),( 0, 0),( 1, 0),( 2, 0),( 3, 0),( 4, 0),
            //     (-3, 1),(-2, 1),(-1, 1),( 0, 1),( 1, 1),( 2, 1),( 3, 1),( 4, 1),
            //     (-3, 2),(-2, 2),(-1, 2),( 0, 2),( 1, 2),( 2, 2),( 3, 2),( 4, 2),
            //                     (-1, 3),( 0, 3),( 1, 3),( 2, 3),
            //                     (-1, 4),( 0, 4),( 1, 4),( 2, 4)
            // };

            // this pattern is used for initializing the nodes of the graph. Given a center at (0,0),
            // centering a Boundary kernel on any of these tiles would encroach on this kernel's path.
            // We initialize by choosing random tiles to center a node on; if we didn't remove these
            // surrounding tiles from the pool, then any new node that fell among them would be necessarily
            // connected to this node in the graph.
            // public static readonly (int,int)[] CenterToCenter =
            // {
            //                     (-3,-6),(-2,-6),(-1,-6),( 0,-6),( 1,-6),( 2,-6),( 3,-6),
            //                     (-3,-5),(-2,-5),(-1,-5),( 0,-5),( 1,-5),( 2,-5),( 3,-5),
            //                     (-3,-4),(-2,-4),(-1,-4),( 0,-4),( 1,-4),( 2,-4),( 3,-4),
            //     (-5,-3),(-4,-3),(-3,-3),(-2,-3),(-1,-3),( 0,-3),( 1,-3),( 2,-3),( 3,-3),( 4,-3),( 5,-3),
            //     (-5,-2),(-4,-2),(-3,-2),(-2,-2),(-1,-2),( 0,-2),( 1,-2),( 2,-2),( 3,-2),( 4,-2),( 5,-2),
            //     (-5,-1),(-4,-1),(-3,-1),(-2,-1),(-1,-1),( 0,-1),( 1,-1),( 2,-1),( 3,-1),( 4,-1),( 5,-1),
            //     (-5, 0),(-4, 0),(-3, 0),(-2, 0),(-1, 0),( 0, 0),( 1, 0),( 2, 0),( 3, 0),( 4, 0),( 5, 0),
            //     (-5, 1),(-4, 1),(-3, 1),(-2, 1),(-1, 1),( 0, 1),( 1, 1),( 2, 1),( 3, 1),( 4, 1),( 5, 1),
            //     (-5, 2),(-4, 2),(-3, 2),(-2, 2),(-1, 2),( 0, 2),( 1, 2),( 2, 2),( 3, 2),( 4, 2),( 5, 2),
            //     (-5, 3),(-4, 3),(-3, 3),(-2, 3),(-1, 3),( 0, 3),( 1, 3),( 2, 3),( 3, 3),( 4, 3),( 5, 3),
            //                     (-3, 4),(-2, 4),(-1, 4),( 0, 4),( 1, 4),( 2, 4),( 3, 4),
            //                     (-3, 5),(-2, 5),(-1, 5),( 0, 5),( 1, 5),( 2, 5),( 3, 5)
            // };
        }

    }


}