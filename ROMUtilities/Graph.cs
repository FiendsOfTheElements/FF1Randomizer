using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMUtilities
{
	public class DirectedGraph<T> where T : IEquatable<T>
	{
		private class Node<U> where U : IEquatable<U>
		{
			public U Item { get; }
			public List<int> Edges { get; }
			public bool Mark { get; set; }

			public Node(U item)
			{
				Item = item;
				Edges = new List<int>();
			}  
		} 

		private readonly List<Node<T>> _nodes = new List<Node<T>>();

		public void AddNode(T node)
		{
			_nodes.Add(new Node<T>(node));
		}

		public void AddNode(T node, List<T> edges)
		{
			AddNode(node);

			int sourceIndex = _nodes.FindIndex(n => n.Item.Equals(node));
			var destinationIndices = edges.Select(edge => _nodes.FindIndex(n => n.Item.Equals(edge)));

			_nodes[sourceIndex].Edges.AddRange(destinationIndices.Except(_nodes[sourceIndex].Edges));
		}

		public void AddEdge(T source, T destination)
		{
			int sourceIndex = _nodes.FindIndex(n => n.Item.Equals(source));
			int destinationIndex = _nodes.FindIndex(n => n.Item.Equals(destination));

			if (!_nodes[sourceIndex].Edges.Contains(destinationIndex))
			{
				_nodes[sourceIndex].Edges.Add(destinationIndex);
			}
		}

		public bool HasCycles()
		{
			foreach (var node in _nodes)
			{
				_nodes.ForEach(n => node.Mark = false);

				if (FindCycle(node))
				{
					return true;
				}
			}

			return false;
		}

		private bool FindCycle(Node<T> node)
		{
			if (node.Mark)
			{
				return true;
			}

			node.Mark = true;

			return node.Edges.Any(edge => FindCycle(_nodes[edge]));
		}
	}
}
