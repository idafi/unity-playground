using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TileMapSystem.Renderer
{
	/// <summary>
	/// Settings container for the TileMapRenderer.
	/// </summary>
	[Serializable]
	public class TileMapRendererSettings
	{
		public int ChunkWidth;				// width of each TileMesh
		public int ChunkHeight;				// height of each TileMesh
		public int FlushThreshold;			// TileMesh pool is flushed when it reaches this size
		
		public string[] CameraBlacklist;	// renderer will not draw to cameras with these names
		
		public TileMapRendererSettings()
		{
			ChunkWidth = 20;
			ChunkHeight = 20;
			FlushThreshold = 50;
			
			CameraBlacklist = new string[]
			{
				"PreRenderCamera"	// used for model/material/etc. previews
			};
		}
	};

	/// <summary>
	/// Renders a TileMap to the scene.
	/// </summary>
	[RequireComponent(typeof(TileMap))]
	[ExecuteInEditMode]
	public class TileMapRenderer : MonoBehaviour
	{
		// tile map is the first opaque item drawn
		const CameraEvent drawEvent = CameraEvent.BeforeForwardOpaque;
		
		[SerializeField]
		TileMapRendererSettings settings;
		
		TileMap map;										// parent tile map
		Dictionary<Camera, CommandBuffer> commandBuffers;	// cameras are acquired and mapped to command buffers as we run
		
		Material material;									// material used for tiles
		TileMeshBuilder meshBuilder;						// mesh builder can and should be reused
		Dictionary<TilePoint, TileMesh> meshPool;			// pool for TileMeshes - flushed when size > flushThreshold
		
		/// <summary>
		/// How many columns of Tiles comprise one rendering chunk.
		/// </summary>
		public int ChunkWidth
		{
			get { return settings.ChunkWidth; }
		}
		
		/// <summary>
		/// How many rows of Tiles comprise one rendering chunk.
		/// </summary>
		public int ChunkHeight
		{
			get { return settings.ChunkHeight; }
		}
		
		int flushThreshold
		{
			get { return settings.FlushThreshold; }
		}
		
		string[] cameraBlacklist
		{
			get { return settings.CameraBlacklist; }
		}
		
		void OnEnable()
		{
			// guarantee settings
			if(settings == null)
			{ settings = new TileMapRendererSettings(); }
		
			map = GetComponent<TileMap>();
			commandBuffers = new Dictionary<Camera, CommandBuffer>();
			
			material = new Material(Shader.Find("Sprites/Default"));	// TODO: user-defined shader/material?
			meshBuilder = new TileMeshBuilder(ChunkWidth, ChunkHeight);
			meshPool = new Dictionary<TilePoint, TileMesh>();

			AttachToCameras();
		}
		
		void OnDisable()
		{
			DetachFromCameras();
			FlushMeshPool();
			UnityEngine.Object.DestroyImmediate(material);
		}
		
		/// <summary>
		/// Fully clears and re-initializes the TileMapRenderer.
		/// </summary>
		public void Rebuild()
		{
			DetachFromCameras();
			FlushMeshPool();

			AttachToCameras();
		}
		
		/// <summary>
		/// Rebuilds any chunks containing the provided tile-space positions.
		/// </summary>
		public void RebuildChunks(IEnumerable<TilePoint> contaningTiles)
		{
			// we're going to filter out all but one position inside a given chunk, so that
			// a chunk containing multiple (or duplicate) positions isn't rebuilt unnecessarily
			List<TilePoint> positions = new List<TilePoint>();

			foreach(TilePoint p in contaningTiles)
			{
				// find the origin of the chunk that contains this tile position
				int chunkX = p.X - (p.X % ChunkWidth);
				int chunkY = p.Y - (p.Y % ChunkHeight);
				TilePoint chunkPos = new TilePoint(chunkX, chunkY);
				
				// add it to the list if it hasn't shown up yet
				if(!positions.Contains(chunkPos))
				{ positions.Add(chunkPos); }
			}
			
			// that was easy. now we just re/build those chunks
			foreach(TilePoint p in positions)
			{
				TileMesh m;
				if(meshPool.TryGetValue(p, out m))
				{ RebuildMesh(p, m); }
			}
		}
		
		void AttachToCameras()
		{
			// detach first, just in case
			DetachFromCameras();
			Camera.onPreRender += Render;
		}
		
		void DetachFromCameras()
		{
			Camera.onPreRender -= Render;
			
			// remove command buffers from cameras before clearing out
			foreach(var pair in commandBuffers)
			{
				Camera cam = pair.Key;
				CommandBuffer buffer = pair.Value;
				
				// camera might not exist anymore
				if(cam)
				{ cam.RemoveCommandBuffer(drawEvent, buffer); }
			}
			
			commandBuffers.Clear();
		}
		
		void FlushMeshPool()
		{
			// free meshes before clearing out
			foreach(TileMesh m in meshPool.Values)
			{ m.Dispose(); }
			
			meshPool.Clear();
		}
		
		void Render(Camera c)
		{
			// flush before drawing, if needed
			if(meshPool.Count > flushThreshold)
			{ FlushMeshPool(); }
		
			DrawChunks(c);
		}
		
		void DrawChunks(Camera c)
		{
			if(!CameraIsBlacklisted(c.name))
			{
				// get command buffer for this camera
				CommandBuffer cBuffer = GetCommandBuffer(c);
				cBuffer.Clear();
				
				// find visible chunk positions
				foreach(TilePoint chunkPos in GetVisibleChunks(c))
				{
					// make sure they're within the tile map's bounds
					if(chunkPos.X < map.Width && chunkPos.Y < map.Height)
					{
						// get a mesh for this position, and draw it to the camera's buffer
						TileMesh mesh = GetMesh(chunkPos);
						mesh.DrawToCommandBuffer(cBuffer, map.Center, map.transform.rotation, material);
					}
				}
			}
		}
	
		bool CameraIsBlacklisted(string name)
		{
			// this could be faster. hopefully user isn't blacklisting like 500 cameras
			return (Array.FindIndex<string>(cameraBlacklist, s => s == name) > -1);
		}
		
		CommandBuffer GetCommandBuffer(Camera c)
		{
			CommandBuffer cBuffer;
			
			if(!commandBuffers.TryGetValue(c, out cBuffer))
			{
				cBuffer = new CommandBuffer();
				cBuffer.name = string.Format("Draw Tile Map ({0})", this.name);
				
				// be sure we haven't already added a buffer to the camera
				c.RemoveCommandBuffer(drawEvent, cBuffer);
				c.AddCommandBuffer(drawEvent, cBuffer);
				
				commandBuffers.Add(c, cBuffer);
			}
			
			return cBuffer;
		}
		
		IEnumerable<TilePoint> GetVisibleChunks(Camera c)
		{
			// get lower and upper bounds of viewport
			float z = map.transform.position.z - c.transform.position.z;
			Vector3 bl = c.ViewportToWorldPoint(new Vector3(0, 0, z));
			Vector3 tr = c.ViewportToWorldPoint(new Vector3(1, 1, z));
			
			// convert those positions to tile space
			TilePoint min = map.ToTileSpace(bl);
			TilePoint max = map.ToTileSpace(tr);
			
			// mod positions to nearest chunk origin
			int minY = min.Y - (min.Y % ChunkHeight);
			int minX = min.X - (min.X % ChunkWidth);
			int maxY = max.Y + (ChunkHeight - (max.Y % ChunkHeight));
			int maxX = max.X + (ChunkWidth - (max.X % ChunkWidth));
			
			// get each chunk position within those bounds
			for(int y = minY; y < maxY; y += ChunkHeight)
			{
				for(int x = minX; x < maxX; x += ChunkWidth)
				{
					// if the point is valid, the chunk is visible; return it
					if(map.HasTile(x, y))
					{ yield return new TilePoint(x, y); }
				}
			}
		}
		
		TileMesh GetMesh(TilePoint chunkPos)
		{
			TileMesh m;
			if(!meshPool.TryGetValue(chunkPos, out m))
			{
				// no mesh for this chunk position; create and build a new one
				m = new TileMesh();
				RebuildMesh(chunkPos, m);
				meshPool.Add(chunkPos, m);
			}

			return m;
		}
		
		void RebuildMesh(TilePoint chunkPos, TileMesh mesh)
		{
			meshBuilder.RebuildTileMesh(mesh, GetTilesForChunk(chunkPos), map.TileSize);
		}

		IEnumerable<Tile> GetTilesForChunk(TilePoint chunkPos)
		{
			// constrain chunk's size so it doesn't exceed the tile map's bounds
			int width = Mathf.Min(ChunkWidth, map.Width - chunkPos.X);
			int height = Mathf.Min(ChunkHeight, map.Height - chunkPos.Y);
			
			// then just load every tile inside the chunk
			for(int tileY = chunkPos.Y; tileY < chunkPos.Y + height; tileY++)
			{
				for(int tileX = chunkPos.X; tileX < chunkPos.X + width; tileX++)
				{
					yield return map.GetTile(tileX, tileY);
				}
			}
		}
	};
}