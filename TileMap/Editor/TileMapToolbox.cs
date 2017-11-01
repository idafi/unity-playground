using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using TileMapSystem;

internal class TileMapToolbox : CED
{
	TileMap map;
	
	ITileMapTool[] tools;
	int currentToolIndex;
	TilePoint lastDragPosition;
	
	ITileMapTool currentTool
	{
		get { return tools[currentToolIndex]; }
	}
	
	IEnumerable<string> toolNames
	{
		get
		{
			foreach(ITileMapTool tool in tools)
			{ yield return tool.Name; }
		}
	}
	
	public TileMapToolbox(TileMap map, SerializedObject data)
	{
		this.map = map;

		tools = new ITileMapTool[] { new TilePainter(map, data), new TileEraser(map, data) };
		currentToolIndex = 0;
	}
	
	public void DrawFields()
	{
		currentToolIndex = PopupField(currentToolIndex, toolNames, "Tool");
		Space();
		
		Label(currentTool.Name, EditorStyles.boldLabel);
		currentTool.DrawFields();
	}
	
	public void DrawScene()
	{
		Vector2 mousePos = GetMousePosition();
		ProcessEvent(mousePos);
		currentTool.DrawScene(mousePos);
	}
	
	Vector2 GetMousePosition()
	{
		Vector2 position = Event.current.mousePosition;
		Ray mRay = HandleUtility.GUIPointToWorldRay(position);

		// vector2 conversion will strip unwanted z value
		return ClampToGrid(mRay.GetPoint(0));
	}
	
	Vector2 ClampToGrid(Vector2 position)
	{
		float size = map.TileSize;

		float x = Mathf.Floor(position.x / size) * size;
		float y = Mathf.Floor(position.y / size) * size;

		return new Vector2(x, y);
	}
	
	void ProcessEvent(Vector2 mousePos)
	{
		Event e = Event.current;

		if(e.button == 0)
		{
			TilePoint tilePos = map.ToTileSpace(mousePos);

			switch(e.type)
			{
				case EventType.MouseDown:
					lastDragPosition = tilePos;
					currentTool.TileClicked(tilePos);
					break;
				case EventType.MouseDrag:
					if(tilePos != lastDragPosition)
					{
						lastDragPosition = tilePos;
						currentTool.TileDragged(tilePos);
					}
					break;
				case EventType.ScrollWheel:
					currentTool.MouseWheel(tilePos, e.delta.y);
					break;
				case EventType.KeyDown:
					KeyEvent();
					break;
			}
		}
	}

	void KeyEvent()
	{
		Event e = Event.current;
		
		switch(e.keyCode)
		{
			case KeyCode.Alpha1:
			case KeyCode.Keypad1:
				SwitchTool(0);
				break;
			case KeyCode.Alpha2:
			case KeyCode.Keypad2:
				SwitchTool(1);
				break;
			case KeyCode.Alpha3:
			case KeyCode.Keypad3:
				SwitchTool(2);
				break;
			case KeyCode.Alpha4:
			case KeyCode.Keypad4:
				SwitchTool(3);
				break;
			case KeyCode.Alpha5:
			case KeyCode.Keypad5:
				SwitchTool(4);
				break;
			case KeyCode.Alpha6:
			case KeyCode.Keypad6:
				SwitchTool(5);
				break;
		}
	}
	
	void SwitchTool(int index)
	{
		currentToolIndex = Mathf.Min(index, tools.Length - 1);
	}
};