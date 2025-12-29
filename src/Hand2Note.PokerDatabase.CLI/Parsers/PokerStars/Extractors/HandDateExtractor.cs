using System.Globalization;
using Hand2Note.PokerDatabase.CLI.Parsers.Core;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Extractors;

public class HandDateExtractor : IHandDataExtractor<DateTime?>
{
    private static readonly Dictionary<string, string> ZoneMap = new()
    {
        { "EET", "+02:00" },
        { "EEST", "+03:00" },
        { "ET", "-05:00" },
        { "EST", "-05:00" },
        { "EDT", "-04:00" }
    };

    public DateTime? Extract(HandParsingContext context)
    {
        var bracketStart = context.HandText.IndexOf('[', StringComparison.Ordinal);
        if (bracketStart == -1)
            return null;

        var bracketEnd = context.HandText.IndexOf(']', bracketStart);
        if (bracketEnd == -1)
            return null;

        var dateStr = context.HandText.Substring(bracketStart + 1, bracketEnd - bracketStart - 1).Trim();
        
        var parts = dateStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
        {
            if (DateTime.TryParse(dateStr, out var fallbackUtc))
                return DateTime.SpecifyKind(fallbackUtc, DateTimeKind.Utc);
            
            return null;
        }

        var date = parts[0];
        var time = parts[1];
        var tz = parts[2];

        if (!ZoneMap.TryGetValue(tz, out var offset))
        {
            if (DateTime.TryParseExact($"{date} {time}", "yyyy/MM/dd H:mm:ss", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var utcGuess))
            {
                return DateTime.SpecifyKind(utcGuess, DateTimeKind.Utc);
            }
            
            if (DateTime.TryParseExact($"{date} {time}", "yyyy/MM/dd HH:mm:ss", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var utcGuess2))
            {
                return DateTime.SpecifyKind(utcGuess2, DateTimeKind.Utc);
            }
            
            return null;
        }

        var normalized = $"{date} {time} {offset}";

        if (DateTime.TryParseExact(normalized, "yyyy/MM/dd H:mm:ss zzz", 
            CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt))
        {
            return dt.ToUniversalTime();
        }

        if (DateTime.TryParseExact(normalized, "yyyy/MM/dd HH:mm:ss zzz", 
            CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt2))
        {
            return dt2.ToUniversalTime();
        }

        return null;
    }
}