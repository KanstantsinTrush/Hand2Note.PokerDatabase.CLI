using Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Extractors;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Steps;

public sealed class ParseState
{
    public long? HandNumber { get; set; }
    public DateTime? Date { get; set; }
    public PlayersExtractionResult? PlayersResult { get; set; }
    public Dictionary<string, string>? PlayerHands { get; set; }
}