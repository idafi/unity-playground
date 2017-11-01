using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains a list of Sprites representing various emotional moods of an IActor.
/// <para>The Custscene will use Wordsboxes, in conjuction with a Script, to dynamically
/// change portrait sprites based on the Script's data.</para>
/// </summary>
[CreateAssetMenu]
public class Portrait : ScriptableObject
{
	[Serializable]
	class SpriteEntry
	{
		public string Name;
		public Sprite Sprite;

		public SpriteEntry()
		{
			Name = null;
			Sprite = null;
		}
	};

	[SerializeField]
	SpriteEntry[] entries;
	Dictionary<string, Sprite> sprites;
	
	/// <summary>
	/// Gets the sprite associated with a particular emotion.
	/// </summary>
	/// <param name="name">The name of the emotion.</param>
	/// <returns>The sprite associated with the emotion.</returns>
	public Sprite GetSprite(string name)
	{
		// who knows when unity's going to recreate this object
		if(sprites == null)
		{
			IndexEntries();
		}

		return sprites[name];
	}

	void IndexEntries()
	{
		sprites = new Dictionary<string, Sprite>();

		foreach(SpriteEntry e in entries)
		{
			sprites.Add(e.Name, e.Sprite);
		}
	}
};