namespace Hand2Note.PokerDatabase.CLI.Parsers.Core;

public class HandParsingContext
{
    public required string HandText { get; init; }
    
    private string[]? _cachedLines;

    public string[] GetLines()
    {
        if (_cachedLines != null)
            return _cachedLines;

        _cachedLines = HandText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return _cachedLines;
    }
}