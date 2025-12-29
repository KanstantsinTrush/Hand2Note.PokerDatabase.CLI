using Hand2Note.PokerDatabase.CLI.Parsers.Core;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Extractors;

public class HandNumberExtractor : IHandDataExtractor<long?>
{
    private const string HandPrefix = "Hand #";

    public long? Extract(HandParsingContext context)
    {
        var handNumberStart = context.HandText.IndexOf(HandPrefix, StringComparison.Ordinal);
        if (handNumberStart == -1)
            return null;

        var handNumberEnd = context.HandText.IndexOf(':', handNumberStart);
        if (handNumberEnd == -1)
            return null;

        var numberStart = handNumberStart + HandPrefix.Length;
        var handNumberStr = context.HandText.Substring(numberStart, handNumberEnd - numberStart).Trim();
        if (!long.TryParse(handNumberStr, out var handNumber))
            return null;

        return handNumber;
    }
}