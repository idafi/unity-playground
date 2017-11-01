using System;
using UnityEngine;

namespace TileMapSystem.Data
{
	/// <summary>
	/// Data-level representation of a single tile.
	/// <para>Its fields are simply indicies into other tile map objects.</para>
	/// </summary>
	[Serializable]
	public struct TileData
	{
		[SerializeField] sbyte tileSet;
		[SerializeField] byte shape;
		[SerializeField] byte sprite;
		[SerializeField] TileFlip flip;
		
		/// <summary>
		/// The index in the corresponding TileMapData object of the tile's TileSet.
		/// </summary>
		public sbyte TileSet
		{
			get { return tileSet; }
		}
		
		/// <summary>
		/// The index in the corresponding TileSet object of the tile's TileShape.
		/// </summary>
		public byte Shape
		{
			get { return shape; }
		}
		
		/// <summary>
		/// The index in the corresponding TileShape object of the tile's Sprite.
		/// </summary>
		public byte Sprite
		{
			get { return sprite; }
		}
		
		/// <summary>
		/// The tile's flip state.
		/// </summary>
		public TileFlip Flip
		{
			get { return flip; }
		}
		
		/// <summary>
		/// Constructs a new TileData instance.
		/// </summary>
		/// <param name="tileSet"></param>
		/// <param name="shape">The index in the corresponding TileMapData object of the new tile's TileSet.</param>
		/// <param name="sprite">The index in the corresponding TileSet object of the new tile's TileShape.</param>
		/// <param name="flip">The index in the corresponding TileShape object of the new tile's Sprite.</param>
		public TileData(sbyte tileSet, byte shape, byte sprite, TileFlip flip)
		{
			this.tileSet = tileSet;
			this.shape = shape;
			this.sprite = sprite;
			this.flip = flip;
		}
		
		public static bool operator ==(TileData a, TileData b)
		{
			return (a.TileSet == b.TileSet) && (a.Shape == b.Shape) &&
				(a.Sprite == b.Sprite) && (a.Flip == b.Flip);
		}
		
		public static bool operator !=(TileData a, TileData b)
		{
			return (a.TileSet != b.TileSet) || (a.Shape != b.Shape) ||
				(a.Sprite != b.Sprite) || (a.Flip != b.Flip);
		}
		
		public override bool Equals(object obj)
		{
			return (this == (TileData)(obj));
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	};
}