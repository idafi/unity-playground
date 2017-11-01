using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using TileMapSystem;

internal static class TileSetImporter
{
	public static TileShape[] Import(Texture2D tex)
	{
		if(tex == null)
		{
			Debug.LogError("couldn't import Tile Set: texture was null");
			return null;
		}

		List<TileShape> shapes = new List<TileShape>();
		IEnumerable<IGrouping<float, Sprite>> groups = GetSprites(tex).OrderBy(s => -s.rect.position.y).GroupBy(s => s.rect.position.x);

		int i = 0;
		foreach(IGrouping<float, Sprite> group in groups)
		{
			string name = string.Format("{0}_{1}", tex.name, i++);
			shapes.Add(new TileShape(name, group.ToArray()));
		}

		return shapes.ToArray();
	}

	static IEnumerable<Sprite> GetSprites(Texture2D tex)
	{
		string path = AssetDatabase.GetAssetPath(tex);
		Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
		Vector2 size = Vector2.zero;

		foreach(Object asset in assets)
		{
			Sprite sprite = asset as Sprite;
			if(sprite != null)
			{
				if(size == Vector2.zero)
				{ size = sprite.rect.size; }

				if(sprite.rect.size == size)
				{
					yield return sprite;
				}
				else
				{
					string msg = string.Format("size mismatch for sprite '{0}'! expected {1} but got {2} - sprite will be ignored",
						sprite.name, size, sprite.rect.size);
					Debug.LogWarning(msg);
				}
			}
		}
	}
};