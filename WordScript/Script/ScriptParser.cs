using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Parser class used by a Script to format raw strings into a Pages.
/// </summary>
public class ScriptParser
{
	/// <summary>
	/// Formats the source string into Pages, used by a Script to feed dialogue to the Wordsbox.
	/// </summary>
	public Page[] Parse(string source)
	{
		// unify line endings
		source = source.Replace("\r\n", "\n");
		
		// split by speaker
		string[] sp = Regex.Split(source, ScriptSyntax.SpeakerPattern, RegexOptions.Multiline);

		if(sp.Length > 1)
		{
			// sp[] is now organized speaker-page-speaker-page; total page count is thus sp.Length / 2
			Page[] pages = new Page[(sp.Length - 1) / 2];
			
			// add data to each page
			for(int s = 1, p = 0; s < sp.Length; s += 2, p++)
			{ pages[p] = new Page(sp[s], ParseWords(sp[s + 1])); }

			return pages;
		}
		else
		{
			Debug.LogError("ScriptParser: expected a speaker declaration");
			return null;
		}
	}

	Word[] ParseWords(string source)
	{
		source = source.Trim();
		
		// .net 4 would let us do this more easily, but unity. oh well.
		// split string up into words, remove anything blank (i.e. extraneous whitespace)
		List<string> sp = new List<string>(Regex.Split(source, ScriptSyntax.WordDelimPattern, RegexOptions.Multiline));
		sp.RemoveAll(x => string.IsNullOrEmpty(x));

		Word[] words = new Word[sp.Count];
		for(int i = 0; i < sp.Count; i++)
		{
			Word w;
			
			// try parsing as portrait or break before treating the string as a plain word
			if(!TryParsePortrait(sp[i], out w) && !TryParseBreak(sp[i], out w))
			{ w = new Word(WordType.Word, sp[i]); }

			words[i] = w;
		}

		return words;
	}

	bool TryParsePortrait(string str, out Word word)
	{
		// is the word formatted as a |portrait|?
		return TryParseWord(str, ScriptSyntax.PortraitPattern, WordType.Portrait, out word);
	}

	bool TryParseBreak(string str, out Word word)
	{
		// is the word just a line break?
		return TryParseWord(str, ScriptSyntax.BreakPattern, WordType.Break, out word);
	}

	bool TryParseWord(string str, string pattern, WordType type, out Word word)
	{
		Match m = Regex.Match(str, pattern);
		word = (m.Success) ? new Word(type, m.Result(@"$1")) : null;

		return m.Success;
	}
};