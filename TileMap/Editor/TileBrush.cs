using UnityEditor;
using UnityEngine;
using TileMapSystem;
using TileMapSystem.Data;
using TileMapSystem.Renderer;

internal abstract class TileBrush : CED, ITileMapTool
{
	protected TileMap map;
	protected SerializedObject data;
	int size;

	public abstract string Name { get; }

	public TileBrush(TileMap map, SerializedObject data)
	{
		this.map = map;
		this.data = data;
		size = 1;
	}

	public virtual void DrawFields()
	{
		size = IntField(size, "Size", min: 1);
	}

	public virtual void DrawScene(Vector2 mousePosition)
	{
		DrawBrushOutline(mousePosition);
		SceneView.RepaintAll();
	}

	public void TileClicked(TilePoint tilePos)
	{
		PaintTiles(tilePos);
	}

	public void TileDragged(TilePoint tilePos)
	{
		PaintTiles(tilePos);
	}

	public void MouseWheel(TilePoint tilePos, float delta)
	{
		Event e = Event.current;

		if(e.control)
		{
			// this is what it is
			int interval = -((int)(delta) / 3);
			size = Mathf.Max(size + interval, 1);

			e.Use();
		}
	}

	protected abstract TileData PaintTile();

	void DrawBrushOutline(Vector2 mousePos)
	{
		float xMin = mousePos.x;
		float yMin = mousePos.y;
		float xMax = xMin + (size * map.TileSize);
		float yMax = yMin + (size * map.TileSize);

		for(int x = 0; x <= size; x++)
		{
			float xPos = xMin + (x * map.TileSize);

			Vector3 start = new Vector3(xPos, yMin);
			Vector3 end = new Vector3(xPos, yMax);

			Handles.DrawLine(start, end);
		}

		for(int y = 0; y <= size; y++)
		{
			float yPos = yMin + (y * map.TileSize);

			Vector3 start = new Vector3(xMin, yPos);
			Vector3 end = new Vector3(xMax, yPos);

			Handles.DrawLine(start, end);
		}
	}

	void PaintTiles(TilePoint basePos)
	{
		TileMapData dataObject = (TileMapData)(data.targetObject);
		int len = size * size;
		TilePoint[] paintedPoints = new TilePoint[len];

		Undo.RecordObject(dataObject, Name);

		for(int y = basePos.Y, t = 0; y < basePos.Y + size; y++)
		{
			for(int x = basePos.X; x < basePos.X + size; x++, t++)
			{
				dataObject.SetTile(x, y, PaintTile());
				paintedPoints[t] = new TilePoint(x, y);
			}
		}

		// there has to be a better way to do this
		TileMapRenderer r = map.GetComponent<TileMapRenderer>();
		if(r)
		{ r.RebuildChunks(paintedPoints); }
	}
};