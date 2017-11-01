using UnityEngine;
using TileMapSystem;

internal interface ITileMapTool
{
	string Name { get; }
	
	void DrawFields();
	void DrawScene(Vector2 mousePos);
	
	void TileClicked(TilePoint tilePos);
	void TileDragged(TilePoint tilePos);
	void MouseWheel(TilePoint tilePos, float delta);
};