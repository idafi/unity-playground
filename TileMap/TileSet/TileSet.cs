using UnityEngine;

namespace TileMapSystem
{
	/// <summary>
	/// Stores a collection of TileShapes.
	/// <para>There's technically nothing stopping you from putting every possible tile configuration in a single TileSet,
	/// but that's really bad organizational practice.</para>
	/// Instead, it's highly recommended that you ensure each sprite sheet is  represented by its own TileSet.
	/// </summary>
	[CreateAssetMenu(fileName = "New Tile Set", menuName = "Tile Set")]
	public class TileSet : ScriptableObject
	{
		[SerializeField]
		TileShape[] shapes;

		void OnEnable()
		{
			if(shapes == null)
			{ shapes = new TileShape[0]; }
		}

		/// <summary>
		/// All TileShapes available to this TileSet.
		/// </summary>
		public TileShape[] Shapes
		{
			get { return shapes; }
			set { shapes = value; }
		}
	};
}