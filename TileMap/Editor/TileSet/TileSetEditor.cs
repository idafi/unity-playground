using UnityEditor;
using UnityEngine;
using TileMapSystem;

[CustomEditor(typeof(TileSet))]
internal class TileSetEditor : Editor
{
	TileSet set
	{
		get { return (TileSet)(target); }
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		Texture2D import = CED.ReferenceField<Texture2D>(null, "Import Sprite Sheet");
		if(import != null)
		{
			TileShape[] imported = TileSetImporter.Import(import);
			Undo.RecordObject(target, "Import Sprite Sheet");
			set.Shapes = imported;
		}

		CED.ArrayField(serializedObject, "shapes", ShapeFields, foldout: false);

		serializedObject.ApplyModifiedProperties();
	}

	void ShapeFields(SerializedProperty shape)
	{
		CED.SetString(shape, propertyPath: "name");
		CED.SetEnum(shape, propertyPath: "colliderType");

		if(CED.GetEnumIndex(shape, propertyPath: "colliderType") == (int)(TileShapeColliderType.Custom))
		{ CED.AutoArrayField(shape, propertyPath: "customColliderPoints", foldout: false, minLength: 3); }
		CED.Space();

		CED.AutoArrayField(shape, propertyPath: "sprites");
	}
};