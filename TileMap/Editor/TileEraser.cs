using UnityEditor;
using TileMapSystem;
using TileMapSystem.Data;

internal class TileEraser : TileBrush
{
	public override string Name
	{
		get { return "Eraser"; }
	}
	
	public TileEraser(TileMap map, SerializedObject data) : base(map, data)
	{
	}
	
	protected override TileData PaintTile()
	{
		return new TileData(-1, 0, 0, TileFlip.None);
	}
};