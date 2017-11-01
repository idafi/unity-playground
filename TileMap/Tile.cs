using UnityEngine;

namespace TileMapSystem
{
	/// <summary>
	/// Runtime representation of a single Tile in the TileMap.
	/// </summary>
	public struct Tile
	{
		/// <summary>
		/// The Tile's position, in tile space.
		/// </summary>
		public readonly TilePoint TilePosition;

		/// <summary>
		/// The tile's flip state.
		/// </summary>
		public readonly TileFlip Flip;

		/// <summary>
		/// The sprite used by the tile.
		/// </summary>
		public readonly Sprite Sprite;

		/// <summary>
		/// The points comprising the tile's collider shape.
		/// </summary>
		public readonly Vector2[] ColliderPoints;
		
		/// <summary>
		/// Constructs a new Tile.
		/// </summary>
		/// <param name="pos">The new Tile's position, in tile space.</param>
		/// <param name="flip">The new tile's flip state.</param>
		/// <param name="spr">The Sprite to be used by the new tile.</param>
		/// <param name="colPoints">the points comprising the tile's collider shape.</param>
		public Tile(TilePoint pos, TileFlip flip, Sprite spr, Vector2[] colPoints)
		{
			Debug.Assert(colPoints != null);
			
			TilePosition = pos;
			Flip = flip;
			Sprite = spr;
			ColliderPoints = colPoints;
		}
	};
}