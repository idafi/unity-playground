/// <summary>
/// Collection of Words associated with a Speaker.
/// </summary>
public class Page
{
	/// <summary>
	/// The Speaker associated with this Page's Words.
	/// </summary>
	public readonly string Speaker;

	/// <summary>
	/// The Words contained by this page.
	/// </summary>
	public readonly Word[] Words;

	/// <summary>
	/// Constructs a new Page from the given speaker name and Words.
	/// </summary>
	/// <param name="speaker">The speaker name to attach to this Page.</param>
	/// <param name="words">The Words used by this page.</param>
	public Page(string speaker, Word[] words)
	{
		Speaker = speaker;
		Words = words;
	}

	public override string ToString()
	{
		return string.Format("{0} ({1} words)", Speaker, Words.Length);
	}
};