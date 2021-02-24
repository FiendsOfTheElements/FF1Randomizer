using System;
using System.Collections.Generic;
using System.Text;

namespace FF1Lib.Procgen
{
	public class EquivalenceNode<T>
	{
		private EquivalenceNode<T> _parent;
		public T Item { get; }

		public EquivalenceNode(T item)
		{
			Item = item;
			_parent = null;
		}

		public bool IsEquivalentTo(EquivalenceNode<T> other)
		{
			EquivalenceNode<T> thisRoot = FindRoot();
			EquivalenceNode<T> otherRoot = other.FindRoot();

			return ReferenceEquals(thisRoot, otherRoot);
		}

		private EquivalenceNode<T> FindRoot()
		{
			if (_parent == null)
			{
				return this;
			}

			EquivalenceNode<T> root = _parent.FindRoot();
			_parent = root;
			return root;
		}

		public void MakeEquivalentTo(EquivalenceNode<T> other)
		{
			EquivalenceNode<T> root = FindRoot();
			root._parent = other;
		}
	}
}
