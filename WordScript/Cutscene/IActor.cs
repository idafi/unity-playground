/// <summary>
/// Represents a single logical actor in a Cutscene.
/// <para>The Cutscene attaches IActors to Wordsboxes, allowing the Wordsbox
/// to dynamically change the IActor's portrait representation based on Script data.</para>
/// </summary>
public interface IActor
{
	/// <summary>
	/// The name to display on Wordsboxes representing this IActor.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// The portrait data for the Wordsbox representing this IActor to use.
	/// </summary>
	Portrait Portrait { get; }
};