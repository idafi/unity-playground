using UnityEditor;
using UnityEngine;
using TileMapSystem;
using TileMapSystem.Renderer;

[CustomEditor(typeof(TileMap))]
internal class TileMapEditor : Editor
{
	SerializedObject data;	// since this is its own object, we have to update it manually
	TileMapToolbox toolbox;	// tile map manipulators are set up here
	Tool lastUnityTool;     // remember last unity tool, so we can restore it when this editor closes
		
	TileMap map
	{
		get { return (TileMap)(target); }
	}

	bool hasData
	{
		get { return (data != null); }
	}
	
	void OnEnable()
	{
		SerializedProperty dataProp = CED.FindProperty(serializedObject, "data");
		if(dataProp.objectReferenceValue != null)
		{
			data = new SerializedObject(dataProp.objectReferenceValue);
			toolbox = new TileMapToolbox(map, data);
			lastUnityTool = Tools.current;
		}
	}
	
	void OnDisable()
	{
		Tools.current = lastUnityTool;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		if(hasData)
		{
			data.Update();

			SizeFields();
			CED.Space();

			CED.AutoArrayField(data, "tileSets");
			CED.Space();

			toolbox.DrawFields();
			Repaint();

			data.ApplyModifiedProperties();
		}
		else
		{
			CED.SetReference(serializedObject, "data");
			CED.HelpBox("TileMap lost its data asset reference! relink it if you can find it; othwerwise, create a new TileMap", MessageType.Error);
		}

		serializedObject.ApplyModifiedProperties();
	}

	void OnSceneGUI()
	{
		// ugh
		if(Event.current.type == EventType.Layout)
		{ Focus(); }
	
		toolbox.DrawScene();
	}
	
	void SizeFields()
	{
		bool rebuild = false;
		SerializedProperty width = CED.FindProperty(data, "width");
		SerializedProperty height = CED.FindProperty(data, "height");
		
		CED.BeginChangeCheck();
		CED.SetInt(width, min: 1);
		CED.SetInt(height, min: 1);
		
		if(CED.EndChangeCheck())
		{
			SerializedProperty tiles = CED.FindProperty(data, "tiles");
			tiles.arraySize = width.intValue * height.intValue;
			rebuild = true;
		}
		
		CED.BeginChangeCheck();
		CED.SetFloat(serializedObject, "tileSize", min: 0.01f);
		if(CED.EndChangeCheck())
		{ rebuild = true; }
	
		if(rebuild)
		{
			TileMapRenderer r = map.GetComponent<TileMapRenderer>();
			if(r)
			{ r.Rebuild(); }
		}
	}
	
	void Focus()
	{
		int id = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);
		HandleUtility.AddDefaultControl(id);

		// ugh
		Tools.current = Tool.None;
	}
};