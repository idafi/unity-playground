/// <summary>
/// Formatted dialogue, organized by Speaker into Pages.
/// </summary>
public class Script
{
	/// <summary>
	/// All pages contained in this Script.
	/// </summary>
	public readonly Page[] Pages;

	/// <summary>
	/// Constructs a new script, parsing Pages from the given source string.
	/// </summary>
	/// <param name="source">The source string from which Pages will be parsed.</param>
	public Script(string source)
	{
		ScriptParser p = new ScriptParser();
		Pages = p.Parse(source);
	}
};