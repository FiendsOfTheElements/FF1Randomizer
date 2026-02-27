using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using static System.Math;
using System.Numerics;


namespace FF1Lib.Procgen
{
        
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
                // mod 64, and wraps correctly for negative numbers as well
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
        public (byte X,byte Y) XY
        {
            get => (_x,_y);

            init
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// lots of possible ways of constructing a Node in
        /// different contexts
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

        ////// Operator overloading
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

        public static Node operator +(Node left, Vector2 right)
        {
            return new Node((byte)(left.X + (int)right.X),(byte)(left.Y + (int)right.Y));
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

        /// Get the x and y delta from this node to another, taking the shortest path with
        /// wraparound to the other side of the map.
        /// Assumes that the map is 64x64, but this could be reused for the overworld or
        /// other dimensions with another field.
        public (int dX,int dY) Delta(Node other)
        {
            int dx = other.X - X;
            if (dx > 32)
            {
                dx -= 64;
            }
            else if (dx < -32)
            {
                dx += 64;
            }
            int dy = other.Y - Y;
            if (dy > 32)
            {
                dy -= 64;
            }
            else if (dy < -32)
            {
                dy += 64;
            }
            
            return (dx,dy);
        }

        // Get the squared pythagorean distance from this node to another
        // (shortest distance with wraparound)
        // Used as a fast weight in Edges.
        public int SqDist(Node other)
        {
            (int dx, int dy) = Delta(other);
            return dx*dx + dy*dy;
        }

        // the pythagorean distance
        public float Dist(Node other)
        {
            return (float)Sqrt(SqDist(other));
        }

        // get the rectilinear distance from this node to another
        public int RectDist(Node other)
        {
            (int dx, int dy) = Delta(other);
            return Abs(dx) + Abs(dy);
        }        

        // a unit vector pointing from this node to another;
        // Unit vectors do not scale the magnitude when used in dot products with other vectors.
        public Vector2 UnitVector(Node other)
        {
            float dist = Dist(other);
            dist = dist == 0? 1 : dist;
            var (dX, dY) = Delta(other);
            return new Vector2(dX/dist, dY/dist);
        }
    }

    // An Edge struct with value semantics, connecting two Nodes.
    // Needs to somehow be agnostic about which node is which
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

        // We use squared distance as the weight, which is less ambiguous than rectilinear distance
        public int Weight
        {
            get => _weight;
            init
            {
                _weight = value;
            }
        }

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

        // Get the other node of the Edge
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

        // not used explicitly but probably needed for .Contains LINQ expressions
        public readonly override bool Equals([NotNullWhen(true)] object obj)
        {
            // cool to use an operator overload here? apparently.
            if (obj is not Edge || this != (Edge)obj)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public readonly override int GetHashCode()
        {
            // the usual one probably works, but there might be reasons to use the other one below
            // since it puts Edges with the same nodes in the same bucket, and doesn't need to hash
            // the Weight
            // return base.GetHashCode();
            return Node1.GetHashCode() + Node2.GetHashCode();
        }

        public static bool operator ==(Edge left, Edge right)
        {
            return left.Nodes == right.Nodes || left.Nodes == (right.Node2,right.Node1);
        }

        public static bool operator !=(Edge left, Edge right)
        {
            return !(left == right);
        }
    }


// maintain a graph of Nodes and Edges
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
            AddRange(edges);
        }

        // init a Graph with a list of Nodes, drawing edges
        // between every pair.
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

        // Get the node that has the most distant closest neighbor.
        public Node MostIsolatedNode(MT19337 rng)
        {
            return Nodes.MaxBy(i => Adjacencies[i].Min(j => j.Weight));
        }

        // Add an Edge. Must also add that Edge to the Adjacencies listings for both Nodes in the Edge.
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

        // Add a bunch of edges.
        public void AddRange(IEnumerable<Edge> edges)
        {
            foreach (Edge e in edges)
            {
                Add(e);
            }
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

        // remove an Edge.
        public bool Remove(Edge edge)
        {
            if (!Edges.Contains(edge))
            {
                return false;
            }
            // first, remove the edge from the Adjacencies entries for both Nodes.
            Node node1 = edge.Node1;
            Node node2 = edge.Node2;
            Adjacencies[node1].Remove(edge);
            Adjacencies[node2].Remove(edge);
            // And remove the Node if it's not part of any Edge in the Graph
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
            // finally remove the Edge from Edges
            return Edges.Remove(edge);
        }

        // Remove a range of Edges.
        public bool RemoveRange(IEnumerable<Edge> edges)
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

        public bool RemoveRange(IEnumerable<Node> nodes)
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

    /// Behaves like a priority queue. There is no Enqueue() method -- it must be
    /// initialized with all of the enqueued values.
    public class MinimumEdgeQueue
    { 
        // set of nodes we can look up edges from
        private HashSet<Node> _scope;

        // each Node gets its own PriorityQueue.
        private Dictionary<Node,PriorityQueue<Edge,int>> _queues;
        
        // constructor; the argument is an Adjacencies dictionary from a Graph
        public MinimumEdgeQueue(Dictionary<Node,HashSet<Edge>> adjacencies)
        {
            _scope = new();
            _queues = new();
            foreach (Node node in adjacencies.Keys)
            {
                _queues[node] = new(adjacencies[node].Select(edge => (edge,edge.Weight)));
            }
        }

        public Edge Dequeue()
        {
            Node nextKey = _scope.MinBy(node => _queues[node].Peek().Weight);
            Edge nextEdge = _queues[nextKey].Dequeue();
            if (_queues[nextKey].Count == 0)
            {
                _scope.Remove(nextKey);
            }
            return nextEdge;

        }

        public void IncreaseScope(Node newNode)
        {
            if (_queues.ContainsKey(newNode))
            {
                if (_queues[newNode].Count != 0)
                {
                    _scope.Add(newNode);
                }
            }
        }

        public int Count()
        {
            return _scope.Sum(node => _queues[node].Count);
        }
    }


}


