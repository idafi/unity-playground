using UnityEngine;

namespace TileMapSystem.Data
{
	// "why does this exist?" unity re/serializes each inspected component multiple times per frame.
	// if the component data includes every single piece of tile data, this leads to unacceptable performance problems.
	// storing tile data in a seperate asset ensures it is only re/serialized when it needs to be
	
	/// <summary>
	/// Backing asset for serialized TileMap data.
	/// </summary>
	public class TileMapData : ScriptableObject
	{
		[SerializeField] int width;
		[SerializeField] int height;
		
		[SerializeField] TileSet[] tileSets;
		[SerializeField] TileData[] tiles;
		
		public int Width
		{
			get { return width; }
		}
		
		public int Height
		{
			get { return height; }
		}
		
		public static TileMapData Create(int width, int height)
		{
			TileMapData data = ScriptableObject.CreateInstance<TileMapData>();
			data.width = Mathf.Max(width, 1);	// ensure valid map width
			data.height = Mathf.Max(height, 1);	// ensure valid map height
			data.tileSets = new TileSet[0];		// easier this way than with a null array
			data.tiles = new TileData[width * height];	// user doesn't directly resize or manipulate tiles array
			
			// initialize tiles to empty
			for(int i = 0; i < data.tiles.Length; i++)
			{ data.tiles[i] = new TileData(-1, 0, 0, TileFlip.None); }
			
			if(width < 1 || height < 1)
			{ Debug.LogWarning(string.Format("TileMapData dimensions ({0}, {1}) are invalid - <0 value(s) will be clamped to 1", width, height)); }

			return data;
		}
		
		public bool HasTile(int x, int y, bool includeEmptyTiles = true)
		{
			int i = GetTileIndex(x, y);
			return ((i > -1) && (includeEmptyTiles || tiles[i].TileSet > -1));
		}
		
		public void SetTile(int x, int y, TileData data)
		{
			int i = GetTileIndex(x, y);
			if(i > -1)
			{ tiles[i] = data; }
		}

		public Tile LoadTile(int x, int y)
		{
			// valid tile position?
			int i = GetTileIndex(x, y);
			if(i > -1)
			{
				TileData data = tiles[i];
				
				// empty tile?
				int setI = data.TileSet;
				if(setI > -1)
				{
					// valid tile set index?
					if(CheckDataIndex(setI, tileSets.Length, x, y, "tile set"))
					{
						TileSet set = tileSets[setI];
						int shapeI = data.Shape;
						
						// valid shape index?
						if(CheckDataIndex(shapeI, set.Shapes.Length, x, y, "shape"))
						{
							TileShape shape = set.Shapes[shapeI];
							int spriteI = data.Sprite;
							
							// valid sprite index?
							if(CheckDataIndex(spriteI, shape.Sprites.Length, x, y, "sprite"))
							{
								// ok, we're good. set up the data and return
								Sprite sprite = shape.Sprites[spriteI];
								TilePoint pos = new TilePoint(x, y);
								Vector2[] colPoints = shape.ColliderPoints;
								
								return new Tile(pos, data.Flip, sprite, colPoints);
							}
						}
					}
				}
			}
			
			return default(Tile);
		}
		
		int GetTileIndex(int x, int y)
		{
			if(IsValidCoordinate(x, y))
			{ return ((y * width) + x); }
			
			return -1;
		}
		
		bool IsValidCoordinate(int x, int y)
		{
			return (x > -1 && x < width) &&
				(y > -1 && y < height);
		}
		
		bool CheckDataIndex(int i, int len, int x, int y, string dataDesc)
		{
			if(i >= len)
			{
				string s = string.Format("couldn't loat tile! {0} index for position ({1}, {2}) was invalid: got {3}, but expected no greater than {4}",
					dataDesc, x, y, i, len);
				Debug.LogError(s);
				
				return false;
			}
			
			return true;
		}
	};
}