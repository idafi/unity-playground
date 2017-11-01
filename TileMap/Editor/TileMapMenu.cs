using UnityEditor;
using TileMapSystem;
using TileMapSystem.Data;

internal static class TileMapMenu
{
	[MenuItem("Tile Map/Create")]
	static void Create()
	{
		TileMapData data = CreateData();

		if(data != null)
		{
			TileMap.Create(data);
		}
	}

	static TileMapData CreateData()
	{
		string path = EditorUtility.SaveFilePanel("Create Tile Map Data", "Assets/", "New Tile Map", "asset");
		int sub = path.IndexOf("Assets/");
		path = path.Substring(sub);

		if(path != "")
		{
			TileMapData data = TileMapData.Create(100, 100);
			AssetDatabase.CreateAsset(data, path);

			return data;
		}

		return null;
	}
};