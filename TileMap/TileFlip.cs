using System;

namespace TileMapSystem
{
	/// <summary>
	/// Describes the flip state of a Tile.
	/// <para>Both the Tile's Sprite and Collider are flipped.</para>
	/// </summary>
	[Flags]
	public enum TileFlip : byte
	{
		None = 0x00,
		Horizontal = 0x01,
		Vertical = 0x02,
		Both = 0x03
	};
}