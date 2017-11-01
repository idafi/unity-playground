using UnityEditor;
using UnityEngine;
using TileMapSystem.Data;

[CustomEditor(typeof(TileMapData))]
internal class TileMapDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GUI.enabled = false;

		CED.SetInt(serializedObject, "width");
		CED.SetInt(serializedObject, "height");
		CED.Space();

		CED.AutoArrayField(serializedObject, "tileSets");
		GUI.enabled = true;
	}
};