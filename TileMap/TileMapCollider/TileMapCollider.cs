using System;
using UnityEngine;

namespace TileMapSystem.Collider
{
	/// <summary>
	/// Settings for the TileMapCollider.
	/// </summary>
	[Serializable]
	public class TileMapColliderSettings
	{
		public int ChunkWidth;
		public int ChunkHeight;
		
		public TileMapColliderSettings()
		{
			ChunkWidth = 20;
			ChunkHeight = 20;
		}
	};
	
	/// <summary>
	/// Adds static-body collision to a TileMap, generated from TileShapes.
	/// </summary>
	public class TileMapCollider : MonoBehaviour
	{
		[SerializeField]
		TileMapColliderSettings settings;
		
		TileMap map;					// the parent TileMap
		GameObject container;			// colliders are parented to a container object, out of the way of the TileMap
		CompositeCollider2D composite;	// TileMap collision is represented by composited polygon colliders
		
		/// <summary>
		/// How many columns of Tile colliders are contained within one container object.
		/// </summary>
		public int ChunkWidth
		{
			get { return settings.ChunkWidth; }
		}
		
		/// <summary>
		/// How many rows of Tile colliders are contained within one container object.
		/// </summary>
		public int ChunkHeight
		{
			get { return settings.ChunkHeight; }
		}
		
		void Start()
		{
			// ensure settings
			if(settings == null)
			{ settings = new TileMapColliderSettings(); }
		
			map = GetComponent<TileMap>();
			CreateContainer();
			CreateComposite();
			GenerateColliders();
		}
		
		// the container is a runtime object, so unity will cleanly nuke it (and thus our collision) for us when the scene unloads
		void CreateContainer()
		{
			container = new GameObject("collision");
			container.transform.position = this.transform.position;
			container.transform.SetParent(this.transform);
		}
		
		void CreateComposite()
		{
			// composite collider requires a rigidbody
			Rigidbody2D body = container.AddComponent<Rigidbody2D>();
			body.bodyType = RigidbodyType2D.Static;
			
			// now that we have one, create the composite and set it up
			composite = container.AddComponent<CompositeCollider2D>();
			composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
			composite.generationType = CompositeCollider2D.GenerationType.Manual;
		}
		
		void GenerateColliders()
		{
			// for now, we'll just make a collider for the whole tile map.
			// this can and should be a lot more efficient, but that's a problem for later
			for(int chunkY = 0; chunkY < map.Height; chunkY += ChunkHeight)
			{
				for(int chunkX = 0; chunkX < map.Width; chunkX += ChunkWidth)
				{
					GenerateChunk(chunkX, chunkY);
				}
			}
			
			// don't forget to manually regenerate the composite
			composite.GenerateGeometry();
		}

		void GenerateChunk(int chunkX, int chunkY)
		{
			// we're going to divide the potentially-several-thousand polygon collider components into their own GameObjects.
			// it's "possible" to have a single GameObject with that many components, but unity will hate it, and us -- especially if it's inspected
			string name = string.Format("chunk_({0}, {1})", chunkX, chunkY);
			GameObject chunkObj = new GameObject(name);
			chunkObj.transform.position = container.transform.position;
			chunkObj.transform.SetParent(container.transform);
			
			// within the chunk object, just add a polygon collider for every collidable tile
			for(int tileY = chunkY; tileY < chunkY + ChunkHeight; tileY++)
			{
				for(int tileX = chunkX; tileX < chunkX + ChunkWidth; tileX++)
				{
					Tile t = map.GetTile(tileX, tileY);
					
					// tile must be collidable (i.e. have 3 or more collider points)
					if(t.ColliderPoints != null && t.ColliderPoints.Length > 0)
					{
						// add an empty polygon collider
						PolygonCollider2D polygon = chunkObj.AddComponent<PolygonCollider2D>();
						polygon.usedByComposite = true;

						// we need to convert the 0-1 ColliderPoints coordinates to world-space
						Vector2[] col = t.ColliderPoints;
						Vector2[] points = new Vector2[col.Length];
						for(int i = 0; i < col.Length; i++)
						{
							bool flipX = (t.Flip == TileFlip.Horizontal || t.Flip == TileFlip.Both);
							bool flipY = (t.Flip == TileFlip.Vertical || t.Flip == TileFlip.Both);
							
							// flip the coordinates if we need to
							float x = (flipX) ? 1 - col[i].x : col[i].x;
							float y = (flipY) ? 1 - col[i].y : col[i].y;
							
							// clamp invalid coordinates
							x = Mathf.Clamp01(x);
							y = Mathf.Clamp01(y);
							
							Vector2 pos = map.ToWorldSpace(t.TilePosition);			// get the world-space position for the tile...
							pos -= (Vector2)(this.transform.position);				// ...but relocate this position relative  to the TileMap's (0,0), rather than the center
							points[i] = pos + (new Vector2(x, y) * map.TileSize);	// offset collider points from that position, scaled to world-space size
						}
						
						// assign converted points to the polygon collider
						polygon.points = points;
					}
				}
			}
		}
	};
}