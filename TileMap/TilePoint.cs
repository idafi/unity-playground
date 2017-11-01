using UnityEngine;

namespace TileMapSystem
{
	// this only exists because Unity doesn't have an (int, int) position structure.
	// if you have your own, feel free to write an implicit conversion
	
	/// <summary>
	/// (int, int) representation of a Tile's position in tile-space.
	/// </summary>
	public struct TilePoint
	{
		/// <summary>
		/// The TilePoint's X coordinate.
		/// </summary>
		public int X;

		/// <summary>
		/// The TilePoint's Y coordinate.
		/// </summary>
		public int Y;
		
		/// <summary>
		/// Constructs a new TilePoint at the given coordinates.
		/// </summary>
		/// <param name="x">The new TilePoint's X coordinate.</param>
		/// <param name="y">The new TilePoint's Y coordinate.</param>
		public TilePoint(int x, int y)
		{
			X = x;
			Y = y;
		}
		
		/// <summary>
		/// Constructs a new TilePoint at the given position.
		/// </summary>
		/// <param name="pos">The position at which to construct the new TilePoint.</param>
		public TilePoint(Vector2 pos)
		{
			// any given position inside a tile "belongs" to the tile that contains it. thus, floor
			X = Mathf.FloorToInt(pos.x);
			Y = Mathf.FloorToInt(pos.y);
		}
		
		public static bool operator ==(TilePoint a, TilePoint b)
		{
			return (a.X == b.X) && (a.Y == b.Y);
		}

		public static bool operator ==(TilePoint a, Vector2 b)
		{
			return (a.X == b.x) && (a.Y == b.y);
		}

		public static bool operator !=(TilePoint a, TilePoint b)
		{
			return (a.X != b.X) || (a.Y != b.Y);
		}

		public static bool operator !=(TilePoint a, Vector2 b)
		{
			return (a.X != b.x) || (a.Y != b.y);
		}

		public static TilePoint operator +(TilePoint a, TilePoint b)
		{
			int x = a.X + b.X;
			int y = a.Y + b.Y;

			return new TilePoint(x, y);
		}

		public static TilePoint operator -(TilePoint a, TilePoint b)
		{
			int x = a.X - b.X;
			int y = a.Y - b.Y;

			return new TilePoint(x, y);
		}

		public static explicit operator Vector2(TilePoint point)
		{
			return new Vector2(point.X, point.Y);
		}
		
		public static explicit operator TilePoint(Vector2 pos)
		{
			return new TilePoint(pos);
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", X, Y);
		}

		// anything that can't cast to TilePoint will will and should toss exceptions
		public override bool Equals(object obj)
		{
			return (this == (TilePoint)(obj));
		}

		// default behavior is fine for structs
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	};
}