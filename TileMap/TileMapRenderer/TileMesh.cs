using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TileMapSystem.Renderer
{
	/// <summary>
	/// Mesh representation of a TileMap "chunk," used by the TileMapRenderer.
	/// </summary>
	public class TileMesh : IDisposable
	{
		// instantiating a new block every frame is pointless; we only need one
		static MaterialPropertyBlock propBlock;
		
		Mesh mesh;					// the actual mesh being drawn
		List<Texture2D> textures;	// textures used by this TileMesh, indexed to the appropriate submeshes
		
		/// <summary>
		/// The unity mesh used by this TileMesh.
		/// </summary>
		public Mesh Mesh
		{
			get { return mesh; }
		}
		
		/// <summary>
		/// All unity textures used by this TileMesh.
		/// </summary>
		public List<Texture2D> Textures
		{
			get { return textures; }
		}
		
		static TileMesh()
		{
			propBlock = new MaterialPropertyBlock();
		}
		
		/// <summary>
		/// Constructs a new, empty TileMesh.
		/// </summary>
		public TileMesh()
		{
			mesh = new Mesh();
			mesh.MarkDynamic();
			
			textures = new List<Texture2D>();
		}
		
		/// <summary>
		/// Frees all Unity resources used by the TileMesh.
		/// </summary>
		public void Dispose()
		{
			if(mesh)
			{
				UnityEngine.Object.DestroyImmediate(mesh);
				mesh = null;	// i forgot what unity voodoo requires this, but i'm pretty sure it exists
			}
			
			textures.Clear();
		}
		
		/// <summary>
		/// Draws the TileMesh to the provided CommandBuffer.
		/// <para>The material's main texture will be replaced with the TileMesh's textures.</para>
		/// </summary>
		public void DrawToCommandBuffer(CommandBuffer buffer, Vector3 position, Quaternion rotation, Material mat)
		{
			if(mesh)
			{
				// set up transform matrix
				Matrix4x4 m = Matrix4x4.TRS(position, rotation, new Vector3(1, 1, 1));
				
				// draw each submesh
				for(int sm = 0; sm < textures.Count; sm++)
				{
					Texture2D tex = textures[sm];
					
					// null texture means empty tiles - we don't draw them, but it's still considered valid
					if(tex)
					{
						propBlock.SetTexture("_MainTex", tex);
						buffer.DrawMesh(mesh, m, mat, sm, -1, propBlock);
					}
				}
			}
			else { Debug.LogError("tried to draw a TileMesh with no mesh"); }
		}
	};
}