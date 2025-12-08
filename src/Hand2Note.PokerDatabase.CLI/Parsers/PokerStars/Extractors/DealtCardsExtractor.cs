using Hand2Note.PokerDatabase.CLI.Parsers.Core;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Extractors;

public class DealtCardsExtractor : IHandDataExtractor<Dictionary<string, string>?>
{
    public Dictionary<string, string>? Extract(HandParsingContext context)
    {
        var playerHands = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var lines = context.GetLines();
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            if (trimmedLine.StartsWith("Dealt to ", StringComparison.Ordinal))
            {
                ParseDealtCards(trimmedLine, playerHands);
            }
        }

        return playerHands.Count > 0 ? playerHands : null;
    }

    private static void ParseDealtCards(string line, Dictionary<string, string> playerHands)
    {
        var startIndex = "Dealt to ".Length;
        var bracketStart = line.IndexOf('[', StringComparison.Ordinal);
        if (bracketStart == -1)
            return;

        var playerName = line.Substring(startIndex, bracketStart - startIndex).Trim();
        if (string.IsNullOrEmpty(playerName))
            return;

        var bracketEnd = line.IndexOf(']', bracketStart);
        if (bracketEnd == -1)
            return;

        var cards = line.Substring(bracketStart + 1, bracketEnd - bracketStart - 1).Trim();
        if (!string.IsNullOrEmpty(cards))
        {
            playerHands[playerName] = cards;
        }
    }
}