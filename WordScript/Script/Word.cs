using System;

/// <summary>
/// A single logical instruction inside a Page, specifying text to print, a portrait to change to, etc.
/// </summary>
public class Word
{
	/// <summary>
	/// The WordType describing how this Word's Value should be interpreted.
	/// </summary>
	public readonly WordType Type;

	/// <summary>
	/// The actual string content of the Word.
	/// </summary>
	public readonly string Value;

	/// <summary>
	/// Constructs a new Word.
	/// </summary>
	/// <param name="type">The Word's WordType.</param>
	/// <param name="value">The Word's string content.</param>
	public Word(WordType type, string value)
	{
		Type = type;
		Value = value;
	}

	public override string ToString()
	{
		string typeName = Enum.GetName(typeof(WordType), Type);
		return string.Format("{0}: {1}", typeName, Value);
	}
};