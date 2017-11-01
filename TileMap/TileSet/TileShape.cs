using System;
using UnityEngine;

namespace TileMapSystem
{
	/// <summary>
	/// Represents possible configurations for a single logical Tile.
	/// When a Tile is loaded, it will randomly select one of its TileShape definition's Sprites,
	/// and -- if collision points are defined -- add those points to the TileMap's collison geometry.
	/// </summary>
	[Serializable]
	public class TileShape
	{
		[SerializeField] string name;
		[SerializeField] Sprite[] sprites;
		[SerializeField] TileShapeColliderType colliderType;
		[SerializeField] Vector2[] customColliderPoints;
		
		/// <summary>
		/// An identifying name for the TileShape.
		/// </summary>
		public string Name
		{
			get { return name; }
		}
		
		/// <summary>
		/// Sprites available to the TileShape.
		/// </summary>
		public Sprite[] Sprites
		{
			get { return sprites; }
		}
		
		/// <summary>
		/// Points of a polygon describing the TileShape's collision representation.
		/// </summary>
		public Vector2[] ColliderPoints
		{
			get
			{
				switch(colliderType)
				{
					case TileShapeColliderType.Auto:
						return autoCollider;
					case TileShapeColliderType.Custom:
						Debug.Assert(customColliderPoints != null);
						Debug.Assert(customColliderPoints.Length > 2);
						return customColliderPoints;
					default:
						return new Vector2[0];
				}
			}
		}
		
		Vector2[] autoCollider
		{
			get
			{
				return new Vector2[]
				{
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(1, 1),
					new Vector2(0, 1)
				};
			}
		}
		
		/// <summary>
		/// Constructs a new, empty TileShape.
		/// </summary>
		public TileShape()
		{
			name = "New Shape";
			sprites = new Sprite[0];
			colliderType = TileShapeColliderType.None;
			customColliderPoints = new Vector2[3];
		}

		/// <summary>
		/// Constructs a new TileShape using the given name and Sprites.
		/// </summary>
		/// <param name="name">The name for the new TileShape.</param>
		/// <param name="sprites">The Sprites for the new TileShape.</param>
		public TileShape(string name, Sprite[] sprites)
		{
			Debug.Assert(sprites != null);
			
			this.name = name;
			this.sprites = sprites;
			colliderType = TileShapeColliderType.None;
			customColliderPoints = new Vector2[3];
		}
	};
}