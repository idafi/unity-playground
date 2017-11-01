using System;

/// <summary>
/// Describes a punctuation mark that modifies the speed at which a Wordsbox will print.
/// <para>Punctuaion marks can include exception strings: e.g., a "Mr." exception will prevent
/// the period in "Mr. Smith" from incurring speed changes.</para>
/// </summary>
[Serializable]
public class Punctuation
{
	/// <summary>
	/// The character used by this punctuation mark.
	/// </summary>
	public char Character;

	/// <summary>
	/// The speed at which the punctuation mark should print, normally.
	/// </summary>
	public float NormalSpeed;

	/// <summary>
	/// The speed at which the punctuation mark should print, if fast-scrolling.
	/// </summary>
	public float FastSpeed;

	/// <summary>
	/// Exception strings -- if a word matches these, the speed change won't occur.
	/// </summary>
	public string[] Exceptions;

	/// <summary>
	/// Checks whether or not the given string is an exception to this punctuation rule.
	/// </summary>
	/// <param name="str">The string to check.</param>
	/// <returns>True if the string qualifies as an exception; false if not.</returns>
	public bool IsException(string str)
	{
		return (Array.FindIndex<string>(Exceptions, x => string.Equals(x, str.Trim(), StringComparison.CurrentCultureIgnoreCase)) > -1);
	}
};