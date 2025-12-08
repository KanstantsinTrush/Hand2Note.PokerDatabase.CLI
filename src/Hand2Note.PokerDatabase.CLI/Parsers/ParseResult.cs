namespace Hand2Note.PokerDatabase.CLI.Parsers;

public class ParseResult
{
    public bool Success { get; init; }
    public string? ErrorReason { get; init; }
    public long? HandNumber { get; init; }
    public int? PlayersFound { get; init; }
    public int? PlayersParsed { get; init; }

    public static ParseResult SuccessResult(long handNumber, int playersFound, int playersParsed)
    {
        return new ParseResult
        {
            Success = true,
            HandNumber = handNumber,
            PlayersFound = playersFound,
            PlayersParsed = playersParsed
        };
    }

    public static ParseResult ErrorResult(string reason, long? handNumber = null)
    {
        return new ParseResult
        {
            Success = false,
            ErrorReason = reason,
            HandNumber = handNumber
        };
    }
}