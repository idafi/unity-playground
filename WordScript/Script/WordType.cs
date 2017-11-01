/// <summary>
/// Specifies how a Word's string content should be interpreted.
/// </summary>
public enum WordType
{
	/// <summary>
	/// A normal word -- the text will be directly printed.
	/// </summary>
	Word,

	/// <summary>
	/// A portrait name -- the Wordsbox will switch to the described portrait.
	/// </summary>
	Portrait,

	/// <summary>
	/// A line break -- word content will be ignored, as the user is prompted to scroll the page.
	/// </summary>
	Break
};