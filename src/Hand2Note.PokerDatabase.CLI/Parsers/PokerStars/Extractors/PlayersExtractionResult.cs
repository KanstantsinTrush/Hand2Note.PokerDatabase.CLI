using Hand2Note.PokerDatabase.CLI.Models;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Extractors;

public class PlayersExtractionResult
{
    public required List<Player> Players { get; init; }
    public required int SeatLinesFound { get; init; }
    public required int PlayersParsed { get; init; }
}