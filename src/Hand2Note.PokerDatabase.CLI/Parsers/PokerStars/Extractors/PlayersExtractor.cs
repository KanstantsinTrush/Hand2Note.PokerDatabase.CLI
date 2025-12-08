using Hand2Note.PokerDatabase.CLI.Models;
using Hand2Note.PokerDatabase.CLI.Parsers.Core;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Extractors;

public class PlayersExtractor : IHandDataExtractor<PlayersExtractionResult>
{
    public PlayersExtractionResult Extract(HandParsingContext context)
    {
        var players = new List<Player>();
        var lines = context.GetLines();
        int seatLinesFound = 0;
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            if (trimmedLine.StartsWith("Seat ", StringComparison.Ordinal) && trimmedLine.Contains(" in chips"))
            {
                seatLinesFound++;
                var player = ParsePlayer(trimmedLine);
                if (player != null)
                {
                    players.Add(player);
                }
            }
        }

        return new PlayersExtractionResult
        {
            Players = players,
            SeatLinesFound = seatLinesFound,
            PlayersParsed = players.Count
        };
    }

    private static Player? ParsePlayer(string line)
    {
        var colonIndex = line.IndexOf(':', StringComparison.Ordinal);
        if (colonIndex == -1)
            return null;

        var afterColon = line[(colonIndex + 1)..].Trim();
        
        var openBracketIndex = afterColon.IndexOf('(', StringComparison.Ordinal);
        if (openBracketIndex == -1)
            return null;

        var name = afterColon[..openBracketIndex].Trim();
        if (string.IsNullOrEmpty(name))
            return null;

        var afterBracket = afterColon[(openBracketIndex + 1)..].TrimStart();
        
        var dollarIndex = afterBracket.IndexOf('$', StringComparison.Ordinal);
        var euroIndex = afterBracket.IndexOf('€', StringComparison.Ordinal);
        
        int currencyIndex;
        if (dollarIndex != -1 && (euroIndex == -1 || dollarIndex < euroIndex))
        {
            currencyIndex = dollarIndex;
        }
        else if (euroIndex != -1)
        {
            currencyIndex = euroIndex;
        }
        else
        {
            return null;
        }

        var currencySymbol = afterBracket[currencyIndex];
        var currency = currencySymbol == '€' ? "€" : "$";
        
        var afterCurrency = afterBracket[(currencyIndex + 1)..];
        
        var spaceIndex = afterCurrency.IndexOf(' ', StringComparison.Ordinal);
        var bracketIndex = afterCurrency.IndexOf(')', StringComparison.Ordinal);
        var rIndex = afterCurrency.IndexOf('\r', StringComparison.Ordinal);
        var nIndex = afterCurrency.IndexOf('\n', StringComparison.Ordinal);
        
        int separatorIndex = -1;
        if (spaceIndex != -1)
            separatorIndex = spaceIndex;
        if (bracketIndex != -1 && (separatorIndex == -1 || bracketIndex < separatorIndex))
            separatorIndex = bracketIndex;
        if (rIndex != -1 && (separatorIndex == -1 || rIndex < separatorIndex))
            separatorIndex = rIndex;
        if (nIndex != -1 && (separatorIndex == -1 || nIndex < separatorIndex))
            separatorIndex = nIndex;
        
        if (separatorIndex == -1)
            return null;

        var stackSizeStr = afterCurrency[..separatorIndex].Trim();
        if (!decimal.TryParse(stackSizeStr, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var stackSize))
            return null;

        return new Player
        {
            Name = name,
            StackSize = stackSize,
            Currency = currency
        };
    }
}