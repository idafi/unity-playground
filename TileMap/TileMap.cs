using UnityEngine;
using TileMapSystem.Collider;
using TileMapSystem.Data;
using TileMapSystem.Renderer;

namespace TileMapSystem
{
	/// <summary>
	/// (X, Y)-keyed representation of uniform-sized Tiles.
	/// </summary>
	public class TileMap : MonoBehaviour
	{
		[SerializeField] float tileSize;
		[SerializeField] TileMapData data;
		
		/// <summary>
		/// Uniform tile size, in world units.
		/// </summary>
		public float TileSize
		{
			get { return tileSize; }
		}
		
		/// <summary>
		/// Width of the TileMap, in tiles.
		/// </summary>
		public int Width
		{
			get { return data.Width; }
		}
		
		/// <summary>
		/// Height of the TileMap, in tiles.
		/// </summary>
		public int Height
		{
			get { return data.Height; }
		}
		
		/// <summary>
		/// Center of the TileMap, in world space.
		/// </summary>
		public Vector3 Center
		{
			get
			{
				float x = (data.Width / 2) * TileSize;
				float y = (data.Height / 2) * TileSize;

				return (this.transform.position - new Vector3(x, y));
			}
		}
		
		/// <summary>
		/// Create a new TileMap, backed by the given data asset.
		/// </summary>
		public static TileMap Create(TileMapData data)
		{
			GameObject go = new GameObject(data.name);
			TileMap map = go.AddComponent<TileMap>();
			map.tileSize = 1.0f;	// default tile size
			map.data = data;

			go.AddComponent<TileMapRenderer>();
			go.AddComponent<TileMapCollider>();

			return map;
		}
		
		/// <summary>
		/// Is there a valid tile at this world-space position?
		/// </summary>
		public bool HasTile(Vector3 worldPosition, bool includeEmptyTiles = true)
		{
			TilePoint t = ToTileSpace(worldPosition);
			return HasTile(t);
		}
		
		/// <summary>
		/// Is there a valid tile at this tile-space position?
		/// </summary>
		public bool HasTile(TilePoint tilePosition)
		{
			return HasTile(tilePosition.X, tilePosition.Y);
		}
		
		/// <summary>
		/// Is there a valid tile at this tile-space position?
		/// </summary>
		public bool HasTile(int x, int y)
		{
			return data.HasTile(x, y);
		}
		
		/// <summary>
		/// Get the Tile containing this world-space position.
		/// </summary>
		public Tile GetTile(Vector3 worldPosition)
		{
			TilePoint t = ToTileSpace(worldPosition);
			return GetTile(t);
		}

		/// <summary>
		/// Get the tile at this tile-space position.
		/// </summary>
		public Tile GetTile(TilePoint tilePosition)
		{
			return GetTile(tilePosition.X, tilePosition.Y);
		}

		/// <summary>
		/// Get the tile at this tile-space position.
		/// </summary>
		public Tile GetTile(int x, int y)
		{
			return data.LoadTile(x, y);
		}
		
		/// <summary>
		/// Converts a Vector3 world-space position to an (int, int) tile-space position.
		/// </summary>
		public TilePoint ToTileSpace(Vector3 position)
		{
			position -= Center;

			int x = Mathf.FloorToInt(position.x / TileSize);
			int y = Mathf.FloorToInt(position.y / TileSize);

			return new TilePoint(x, y);
		}

		/// <summary>
		/// Converts an (int, int) tile-space position to a Vector3 world-space position.
		/// </summary>
		public Vector3 ToWorldSpace(TilePoint tilePosition)
		{
			float x = tilePosition.X * TileSize;
			float y = tilePosition.Y * TileSize;
			Vector3 pos = new Vector3(x, y);

			return pos + Center;
		}
	};
}