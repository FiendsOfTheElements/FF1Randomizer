using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FF1Lib.Procgen
{
	class BSPTreeEngine: IMapGeneratorEngine
	{
		internal const int MAX_LEAF_SIZE = 20;
		internal const int MIN_LEAF_SIZE = 8;
		internal const int MIN_ROOM_HEIGHT = 5;
		internal const int MIN_ROOM_WIDTH = 6; 
		internal const int MAX_HALLWAY_WIDTH = 3; // don't make hallway width more than a min room dimension.


		private List<BSPMapNode> all_nodes;
		private HashSet<BSPMapNode> leaf_nodes;
		private BSPMapNode root;

		public BSPTreeEngine()
		{
		}

		public CompleteMap Generate(MT19337 rng, MapRequirements reqs)
		{
			var map = new Map((byte) reqs.Barrier);

			GenerateTree(rng);

			foreach (var roomspec in reqs.Rooms)
			{
				// randomly descend rooms until you find one that fits
				// var leaf_that_fits = RandomLeafWhere(rng, leaf => leaf.Width >= roomspec.Width && leaf.Height >= roomspec.Height);
				// if (leaf_that_fits == null)
				//    return null;
				// leaf_that_fits.WalkableSpace = Rectangle.Empty; // don't generate one
				// TODO something
			}

			root.GenerateRooms(rng);

			IEnumerable<Rectangle> all_the_rectangles = all_nodes.
				Where(leaf => leaf.WalkableSpace != null).
				Select(leaf => (Rectangle) leaf.WalkableSpace).
				Concat(
					all_nodes.
						Where(leaf => leaf.Hallways != null).
						SelectMany(leaf => leaf.Hallways)
				);

			foreach (var rect in all_the_rectangles)
			{
				// TODO something smarter
				map.Fill((rect.Left, rect.Top), (rect.Width, rect.Height), reqs.Floor);
			}

			var smoothIterations = 0;
			while (MapHelper.SmoothFilter(map, reqs.Barrier, reqs.Floor))
				smoothIterations++;

			Point entranceLocation = PointInAnyRandomRoom(rng);
			map[entranceLocation.Y, entranceLocation.X] = (byte)Tile.WarpUp;
			

			return new CompleteMap
			{
				Map = map,
				Entrance = new Coordinate((byte)entranceLocation.X, (byte)entranceLocation.Y, CoordinateLocale.Standard),
				Requirements = reqs
			};

		}

		private void GenerateTree(MT19337 rng)
		{
			root = new BSPMapNode(0, 0, MapRequirements.Width, MapRequirements.Height);
			all_nodes = new List<BSPMapNode> {root};
			leaf_nodes = new HashSet<BSPMapNode> {root};

			while (true)
			{
				var new_leafs = new List<BSPMapNode>();

				foreach (var split_leaf in leaf_nodes.ToList().
					Where(l => l.Width > MAX_LEAF_SIZE || l.Height > MAX_LEAF_SIZE || rng.Between(0, 3) < 3).  // always split if too big, or on a 3/4 chance.
					Where(l => l.Split(rng))) // only select leafs that actually successfully split
				{
					// if we did split, add the new leafs to the list
					new_leafs.Add(split_leaf.LeftChild);
					new_leafs.Add(split_leaf.RightChild);

					leaf_nodes.Remove(split_leaf);
				}

				if (new_leafs.Count == 0)
					break;

				all_nodes.AddRange(new_leafs);
				leaf_nodes.UnionWith(new_leafs);
			}
		}
		
		private Point PointInAnyRandomRoom(MT19337 rng)
		{
			var some_room = (Rectangle)root.GetRandomRoom(rng);
			return new Point(some_room.X + rng.Between(1,some_room.Width - 1), some_room.Y + rng.Between(1, some_room.Height - 1));
		}

		private BSPMapNode RandomLeafWhere(MT19337 rng, Func<BSPMapNode, Boolean> predicate)
		{
			var selected = leaf_nodes.Where(predicate).ToList();

			if (selected.Count == 0)
				return null;

			return selected.PickRandom(rng);
		}

	}

	class BSPMapNode
	{
 
		public int X, Y, Height, Width;
		public BSPMapNode LeftChild, RightChild;
		public Rectangle? WalkableSpace = null;
		public List<Rectangle> Hallways = null;

 
	    public BSPMapNode(int x, int y, int width, int height)
	    {
		    X = x;
		    Y = y;
		    Width = width;
		    Height = height;
	    }
 
		public bool Split(MT19337 rng)
		{
	        // begin splitting the leaf into two children
	        if (LeftChild != null || RightChild != null)
	            return false; // we're already split! Abort!
	 
	        // determine direction of split
	        // if the width is >25% larger than height, we split vertically
	        // if the height is >25% larger than the width, we split horizontally
	        // otherwise we split randomly
			bool split_horizontally;
	        if (Width > Height && Width / Height >= 1.25)
	            split_horizontally = false;
	        else if (Height > Width && Height / Width >= 1.25)
	            split_horizontally = true;
	        else
		        split_horizontally = rng.Between(0,1) == 0;
	 
	        int max = (split_horizontally ? Height : Width) - BSPTreeEngine.MIN_LEAF_SIZE; // determine the maximum height or width
	        if (max <= BSPTreeEngine.MIN_LEAF_SIZE)
	            return false; // the area is too small to split any more...
	 
	        int split = rng.Between(BSPTreeEngine.MIN_LEAF_SIZE, max); // determine where we're going to split
	 
	        // create our left and right children based on the direction of the split
	        if (split_horizontally)
	        {
	            LeftChild = new BSPMapNode(X, Y, Width, split);
	            RightChild = new BSPMapNode(X, Y + split, Width, Height - split);
	        }
	        else
	        {
	            LeftChild = new BSPMapNode(X, Y, split, Height);
	            RightChild = new BSPMapNode(X + split, Y, Width - split, Height);
	        }

	        return true; // split successful!
	    }

		public void GenerateRooms(MT19337 rng)
		{
			// this function generates all the rooms and hallways for this BSPMapNode and all of its children.
			if (LeftChild == null && RightChild == null)
			{
				if (WalkableSpace != null)
					return;

				// the room can be between 3 x 3 tiles to the size of the leaf - 2.
				var roomWidth = rng.Between(BSPTreeEngine.MIN_ROOM_WIDTH, Width - 2);
				var roomHeight = rng.Between(BSPTreeEngine.MIN_ROOM_HEIGHT, Height - 2);

				// place the room within the BSPMapNode, but don't put it right 
				// against the side of the BSPMapNode (that would merge rooms together)
				var roomStartX = rng.Between(1, Width - roomWidth - 1);
				var roomStartY = rng.Between(1, Height - roomHeight - 1);
				WalkableSpace = new Rectangle(X + roomStartX, Y + roomStartY, roomWidth, roomHeight);
				return;
			}

			// subleafs:
			LeftChild?.GenerateRooms(rng);
			RightChild?.GenerateRooms(rng);

			// both leafs exist. we know their GetRandomRoom shouldn't return null, because we just called GenerateRooms.
			if (LeftChild != null && RightChild != null)
				MakeHallway(rng, (Rectangle) LeftChild.GetRandomRoom(rng), (Rectangle) RightChild.GetRandomRoom(rng));
		}

		private void MakeHallway(MT19337 rng, Rectangle l, Rectangle r)
		{
		    // now we connect these two rooms together with hallways.
		    // this looks pretty complicated, but it's just trying to figure out which point is where and then either draw a straight line, or a pair of lines to make a right-angle to connect them.
		    // you could do some extra logic to make your halls more bendy, or do some more advanced things if you wanted.
		 
		    Hallways = new List<Rectangle>();

			int hallway_width = rng.Between(0, 4) == 4
				? rng.Between(1, BSPTreeEngine.MAX_HALLWAY_WIDTH)
				: BSPTreeEngine.MAX_HALLWAY_WIDTH;

			var point1 = new Point(rng.Between(l.Left, l.Right - hallway_width), rng.Between(l.Top, l.Bottom - hallway_width));
			var point2 = new Point(rng.Between(r.Left, r.Right - hallway_width), rng.Between(r.Top, r.Bottom - hallway_width));
			int x_difference = point2.X - point1.X;
			int y_difference = point2.Y - point1.Y;


		    if (x_difference < 0)
		    {
		        if (y_difference < 0)
		        {
		            if (rng.Between(0,1) == 0)
		            {
		                Hallways.Add(new Rectangle(point2.X, point1.Y, Math.Abs(x_difference), hallway_width));
		                Hallways.Add(new Rectangle(point2.X, point2.Y, hallway_width, Math.Abs(y_difference)));
		            }
		            else
		            {
		                Hallways.Add(new Rectangle(point2.X, point2.Y, Math.Abs(x_difference), hallway_width));
		                Hallways.Add(new Rectangle(point1.X, point2.Y, hallway_width, Math.Abs(y_difference)));
		            }
		        }
		        else if (y_difference > 0)
		        {
		            if (rng.Between(0,1) == 0)
		            {
		                Hallways.Add(new Rectangle(point2.X, point1.Y, Math.Abs(x_difference), hallway_width));
		                Hallways.Add(new Rectangle(point2.X, point1.Y, hallway_width, Math.Abs(y_difference)));
		            }
		            else
		            {
		                Hallways.Add(new Rectangle(point2.X, point2.Y, Math.Abs(x_difference), hallway_width));
		                Hallways.Add(new Rectangle(point1.X, point1.Y, hallway_width, Math.Abs(y_difference)));
		            }
		        }
		        else // if (h == 0)
		        {
		            Hallways.Add(new Rectangle(point2.X, point2.Y, Math.Abs(x_difference), hallway_width));
		        }
		    }
		    else if (x_difference > 0)
		    {
		        if (y_difference < 0)
		        {
		            if (rng.Between(0,1) == 0)
		            {
		                Hallways.Add(new Rectangle(point1.X, point2.Y, Math.Abs(x_difference), hallway_width));
		                Hallways.Add(new Rectangle(point1.X, point2.Y, hallway_width, Math.Abs(y_difference)));
		            }
		            else
		            {
		                Hallways.Add(new Rectangle(point1.X, point1.Y, Math.Abs(x_difference), hallway_width));
		                Hallways.Add(new Rectangle(point2.X, point2.Y, hallway_width, Math.Abs(y_difference)));
		            }
		        }
		        else if (y_difference > 0)
		        {
		            if (rng.Between(0,1) == 0)
		            {
		                Hallways.Add(new Rectangle(point1.X, point1.Y, Math.Abs(x_difference), hallway_width));
		                Hallways.Add(new Rectangle(point2.X, point1.Y, hallway_width, Math.Abs(y_difference)));
		            }
		            else
		            {
		                Hallways.Add(new Rectangle(point1.X, point2.Y, Math.Abs(x_difference), hallway_width));
		                Hallways.Add(new Rectangle(point1.X, point1.Y, hallway_width, Math.Abs(y_difference)));
		            }
		        }
		        else // if (y_difference == 0)
		        {
		            Hallways.Add(new Rectangle(point1.X, point1.Y, Math.Abs(x_difference), hallway_width));
		        }
		    }
		    else // if (x_difference == 0)
		    {
		        if (y_difference < 0)
		        {
		            Hallways.Add(new Rectangle(point2.X, point2.Y, hallway_width, Math.Abs(y_difference)));
		        }
		        else if (y_difference > 0)
		        {
		            Hallways.Add(new Rectangle(point1.X, point1.Y, hallway_width, Math.Abs(y_difference)));
		        }
		    }

		}

		public Rectangle? GetRandomRoom(MT19337 rng)
		{
			if (WalkableSpace != null)
				return WalkableSpace;

			var leftRoom = LeftChild.GetRandomRoom(rng);
			var rightRoom = RightChild.GetRandomRoom(rng);

			if (leftRoom == null)
			{
				if (rightRoom == null)
					return null;
				return rightRoom;
			}

			if (rightRoom == null)
				return leftRoom;

			// got both
			return (rng.Between(0,1) == 0) ? leftRoom : rightRoom;
		}

	}
}
