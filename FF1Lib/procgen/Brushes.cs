using System;
using System.Collections.Generic;
using System.Text;

namespace FF1Lib.Procgen
{
	enum BrushPart
	{
		topleft, topcenter, topright,
		midleft, interior, midright,
		bottomleft, bottomcenter, bottomright
	}

	interface IBrush
	{
		Tile TileForPart(BrushPart part);
	}

	/// <summary>
	/// A brush that paints one tile out of nine depending on the requested BrushPart.
	/// </summary>
	class MultitileBrush : IBrush
	{
		private readonly Dictionary<BrushPart, Tile> partTiles;

		/// <summary>
		/// Make a new MultitileBrush. Tiles must be in order of <see cref="BrushPart"/> enum values.
		/// </summary>
		public MultitileBrush (Tile[] tiles)
		{
			int num_of_brush_parts = Enum.GetValues(typeof(BrushPart)).Length;
			if (tiles.Length != num_of_brush_parts)
				throw new ArgumentOutOfRangeException(nameof(tiles), $"Length of 'tiles' array must be equal to the number of BrushParts ({num_of_brush_parts}).");

			for (int i = 0; i < num_of_brush_parts; i++)
				partTiles.Add((BrushPart) i, tiles[i]);
		}

		public MultitileBrush(Dictionary<BrushPart, Tile> tileDictionary)
		{
			foreach (int value in Enum.GetValues(typeof(BrushPart)))
			{
				if (!tileDictionary.ContainsKey((BrushPart) value))
					throw new ArgumentException(nameof(tileDictionary), $"Tile Dictionary must contain a value for every BrushPart.");
			}

			partTiles = tileDictionary;
		}

		public Tile TileForPart(BrushPart part)
		{
			return partTiles[part];
		}
	};

	/// <summary>
	/// A brush that always paints the same one tile.
	/// </summary>
	class SimpleBrush : IBrush
	{
		public Tile tile;

		public SimpleBrush(Tile tile)
		{
			this.tile = tile;
		}

		public Tile TileForPart(BrushPart part)
		{
			return tile;
		}
	}
}
