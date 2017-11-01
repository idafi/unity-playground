using UnityEditor;
using UnityEngine;
using TileMapSystem;
using TileMapSystem.Renderer;

[CustomEditor(typeof(TileMapRenderer))]
internal class TileMapRendererEditor : Editor
{
	TileMap map;
	bool highlightChunks;

	TileMapRenderer renderer
	{
		get { return (TileMapRenderer)(target); }
	}

	void OnEnable()
	{
		map = renderer.GetComponent<TileMap>();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		highlightChunks = CED.BoolField(highlightChunks, "Highlight Chunks");
		CED.Space();

		CED.BeginChangeCheck();
		CED.SetInt(serializedObject, "settings.ChunkWidth", min: 1);
		CED.SetInt(serializedObject, "settings.ChunkHeight", min: 1);

		if(CED.EndChangeCheck())
		{ renderer.Rebuild(); }

		CED.SetInt(serializedObject, "settings.FlushThreshold", min: 1);
		CED.AutoArrayField(serializedObject, "settings.CameraBlacklist");

		serializedObject.ApplyModifiedProperties();
	}

	void OnSceneGUI()
	{
		if(highlightChunks)
		{
			for(int y = 0; y < map.Height; y += renderer.ChunkHeight)
			{
				for(int x = 0; x < map.Width; x += renderer.ChunkWidth)
				{
					int l = x;
					int b = y;
					int r = Mathf.Min(x + renderer.ChunkWidth, map.Width);
					int t = Mathf.Min(y + renderer.ChunkHeight, map.Height);

					Vector3 bl = map.ToWorldSpace(new TilePoint(l, b));
					Vector3 br = map.ToWorldSpace(new TilePoint(r, b));
					Vector3 tr = map.ToWorldSpace(new TilePoint(r, t));
					Vector3 tl = map.ToWorldSpace(new TilePoint(l, t));

					Handles.color = Color.cyan;
					Handles.DrawLines(new Vector3[]
					{
						bl, br,
						br, tr,
						tr, tl,
						tl, bl
					});
				}
			}
		}
	}
}