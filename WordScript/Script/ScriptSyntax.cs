/// <summary>
/// Regex syntax for parsing Script tokens.
/// <summary>
public static class ScriptSyntax
{
	// line ending with colon, containing no whitespace or extra colons
	public const string SpeakerPattern = @"^([^:\s]+):$";
	
	// either newline, or any character except non-newline whitespace
	public const string WordDelimPattern = @"[\f\r\t\v\x85\p{Z}]+|(\n)+";	
	
	// a set of |characters| bounded by vertical bars, containing no whitespace
	public const string PortraitPattern = @"^\|([^\|\s]+)\|$";
	
	// a completely blank line
	public const string BreakPattern = @"^(\n)$";
};