namespace TileMapSystem
{
	/// <summary>
	/// Describes how a Tile's collider should be generated.
	/// </summary>
	public enum TileShapeColliderType
	{
		/// <summary>
		/// Don't generate a collider for this tile.
		/// </summary>
		None,

		/// <summary>
		/// Automatically generate a square collider for this tile.
		/// </summary>
		Auto,

		/// <summary>
		/// Use a user-defined polygon for this tile's collider.
		/// </summary>
		Custom
	};
}