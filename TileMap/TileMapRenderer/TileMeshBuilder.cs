using System.Collections.Generic;
using UnityEngine;

namespace TileMapSystem.Renderer
{
	/// <summary>
	/// (Re)builds TileMeshes using arbitrary collections of Tiles.
	/// </summary>
	public class TileMeshBuilder
	{
		// it's easiest if we organize tiles sharing a texture into submeshes
		struct Submesh
		{
			public Texture2D Texture;
			public List<int> Triangles;
			
			public void Clear()
			{
				Texture = null;
				Triangles.Clear();
			}
			
			public void AddTriangle(int vertexA, int vertexB, int vertexC)
			{
				Triangles.Add(vertexA);
				Triangles.Add(vertexB);
				Triangles.Add(vertexC);
			}
		};
		
		// this needs to be fast, so we'll stick with fixed-size arrays
		Vector3[] vertices;
		int vertexCount;
		
		Vector2[] uvs;
		int uvCount;
		
		Submesh[] submeshes;
		int submeshCount;
		
		/// <summary>
		/// Constructs a new TileMeshBuilder, targeting the given chunk dimensions.
		/// </summary>
		/// <param name="chunkWidth">The width at which to build chunks.</param>
		/// <param name="chunkHeight">The height at which to build chunks.</param>
		public TileMeshBuilder(int chunkWidth, int chunkHeight)
		{
			int vertexCount = (chunkWidth * chunkHeight) * 4;
			
			vertices = new Vector3[vertexCount];
			uvs = new Vector2[vertexCount];
			submeshes = new Submesh[30];	// oh no, not a magic constant! maybe user should set this somewhere?
			
			for(int i = 0; i < submeshes.Length; i++)
			{ submeshes[i].Triangles = new List<int>(); }
		}
		
		/// <summary>
		/// Rebuilds a TileMesh, using the given Tiles and tile size.
		/// </summary>
		public void RebuildTileMesh(TileMesh mesh, IEnumerable<Tile> tiles, float tileSize)
		{
			// idiot user might be trying to rebuild a disposed TileMesh
			if(mesh.Mesh)
			{
				// clear submeshes
				for(int i = 0; i < submeshCount; i++)
				{ submeshes[i].Clear(); }
			
				// reset for rebuild
				vertexCount = 0;
				uvCount = 0;
				submeshCount = 0;
				
				// build up our vertex/uv/submesh arrays, then assign them to the TileMesh
				BuildTiles(tiles, tileSize);
				RebuildMesh(mesh.Mesh);
				RebuildTextures(mesh.Textures);
			}
			else { Debug.LogError("tried to rebuild a TileMesh with no mesh (was it already disposed?)"); }
		}
		
		void BuildTiles(IEnumerable<Tile> tiles, float tileSize)
		{
			foreach(Tile t in tiles)
			{ BuildTile(t, tileSize); }
		}
		
		void RebuildMesh(Mesh m)
		{
			// things go better if we clear the mesh and tell unity we're rebuilding often
			m.Clear();
			m.MarkDynamic();
			
			// copy our newly built vertex/uv arrays to the mesh
			m.vertices = vertices;
			m.uv = uvs;
			
			// set up submeshes
			m.subMeshCount = submeshCount;
			for(int i = 0; i < submeshCount; i++)
			{ m.SetTriangles(submeshes[i].Triangles, i); }
		}
		
		void RebuildTextures(List<Texture2D> textures)
		{
			// we'll just clear out and repopulate the TileMesh's texture list
			textures.Clear();
			for(int i = 0; i < submeshCount; i++)
			{ textures.Add(submeshes[i].Texture); }
		}
		
		void BuildTile(Tile t, float tileSize)
		{
			AddTileVertices(t.TilePosition, tileSize);
			AddTileUVs(t.Sprite, t.Flip);
			AddTileTriangles((t.Sprite) ? t.Sprite.texture : null);	// empty tiles won't have a valid Sprite reference
		}
		
		void AddTileVertices(TilePoint tilePos, float size)
		{
			// positions are local to the TileMesh, but sized in world space
			float x = (float)(tilePos.X) * size;
			float y = (float)(tilePos.Y) * size;
			
			vertices[vertexCount++] = new Vector3(x, y);				// left/bottom
			vertices[vertexCount++] = new Vector3(x + size, y);			// right/bottom
			vertices[vertexCount++] = new Vector3(x + size, y + size);	// right/top
			vertices[vertexCount++] = new Vector3(x, y + size);			// left/top
		}
		
		void AddTileUVs(Sprite spr, TileFlip flip)
		{
			if(spr)
			{
				// we'll have to get uvs for the sprite ourselves
				Texture2D tex = spr.texture;
				Rect rect = spr.rect;
				
				// figure out if this tile is flipped
				bool flipX = (flip == TileFlip.Horizontal || flip == TileFlip.Both);
				bool flipY = (flip == TileFlip.Vertical || flip == TileFlip.Both);
				
				// get the sprite's pixel-space bounds on the texture
				float xMin = (flipX) ? rect.xMax : rect.xMin;
				float yMin = (flipY) ? rect.yMax : rect.yMin;
				float xMax = (flipX) ? rect.xMin : rect.xMax;
				float yMax = (flipY) ? rect.yMin : rect.yMax;
				
				// convert those to 0-1 fractions
				float l = xMin / tex.width;
				float b = yMin / tex.height;
				float r = xMax / tex.width;
				float t = yMax / tex.height;
				
				// and that gives us our uv coordinates
				uvs[uvCount++] = new Vector2(l, b);
				uvs[uvCount++] = new Vector2(r, b);
				uvs[uvCount++] = new Vector2(r, t);
				uvs[uvCount++] = new Vector2(l, t);
			}
			else
			{
				// for an empty tile, just toss some "default" uvs
				// no texture is actually going to be mapped, but unity can't build the mesh without them
				uvs[uvCount++] = new Vector2(0, 0);
				uvs[uvCount++] = new Vector2(1, 0);
				uvs[uvCount++] = new Vector2(1, 1);
				uvs[uvCount++] = new Vector2(0, 1);
			}
		}
		
		void AddTileTriangles(Texture2D tex)
		{
			// try and find an extant submesh using this texture
			// "why not use List.Find" it causes unnecessary GC allocs, which can nuke performance if called rapidly
			int submesh = -1;
			for(int i = 0; i < submeshCount; i++)
			{
				if(submeshes[i].Texture == tex)
				{
					submesh = i;
					break;
				}
			}
			
			// if no submesh was found, set up a new one for this texture
			if(submesh < 0)
			{
				submesh = submeshCount++;
				submeshes[submesh].Texture = tex;
			}
			
			// set up the triangles
			int a = vertexCount - 4;             // left/bottom
			int b = vertexCount - 3;             // right/bottom
			int c = vertexCount - 1;             // left/top
			submeshes[submesh].AddTriangle(a, b, c);

			a = vertexCount - 2;                 // right/top
			b = vertexCount - 1;                 // left/top
			c = vertexCount - 3;                 // right/bottom
			submeshes[submesh].AddTriangle(a, b, c);
		}
	};
}