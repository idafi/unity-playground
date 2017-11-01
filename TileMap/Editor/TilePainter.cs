using System;
using System.Collections.Generic;
using UnityEditor;
using TileMapSystem;
using TileMapSystem.Data;

internal class TilePainter : TileBrush
{
	int set;
	int shape;
	TileFlip flip;
	
	public override string Name
	{
		get { return "Painter"; }
	}
	
	public TilePainter(TileMap map, SerializedObject data) : base(map, data)
	{
	}
	
	public override void DrawFields()
	{
		base.DrawFields();

		SerializedProperty sets = FindProperty(data, "tileSets");
		if(sets.arraySize > 0)
		{
			set = PopupField(set, GetTileSetNames(), "Tile Set");

			SerializedProperty setP = sets.GetArrayElementAtIndex(set);
			if(setP.objectReferenceValue != null)
			{
				SerializedObject obj = new SerializedObject(setP.objectReferenceValue);
				if(FindProperty(obj, "shapes").arraySize > 0)
				{
					shape = PopupField(shape, GetShapeNames(), "Shape");
					flip = (TileFlip)(EnumField((Enum)(flip), "Flip"));
				}
			}
		}
	}
	
	protected override TileData PaintTile()
	{
		return new TileData((sbyte)(set), (byte)(shape), (byte)(GetRandomSprite()), flip);
	}
	
	IEnumerable<string> GetTileSetNames()
	{
		SerializedProperty sets = FindProperty(data, "tileSets");
		
		for(int i = 0; i < sets.arraySize; i++)
		{
			yield return sets.GetArrayElementAtIndex(i).displayName;
		}
	}
	
	IEnumerable<string> GetShapeNames()
	{
		string path = string.Format("tileSets[{0}].shapes", set);
		SerializedProperty shapes = FindProperty(data, path);
		
		for(int i = 0; i < shapes.arraySize; i++)
		{
			yield return GetString(shapes.GetArrayElementAtIndex(i), "name");
		}
	}
	
	int GetRandomSprite()
	{
		string path = string.Format("tileSets[{0}].shapes[{1}].sprites", set, shape);
		SerializedProperty sprites = FindProperty(data, path);
		
		return UnityEngine.Random.Range(0, sprites.arraySize);
	}
};