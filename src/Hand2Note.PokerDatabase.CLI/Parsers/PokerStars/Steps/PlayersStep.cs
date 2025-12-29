using Hand2Note.PokerDatabase.CLI.Parsers.Core;
using Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Extractors;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Steps;

public sealed class PlayersStep : IParseStep<ParseState>
{
    public bool Execute(HandParsingContext context, ParseState state, out string? error)
    {
        var extractor = new PlayersExtractor();
        var playersResult = extractor.Extract(context);

        if (playersResult.Players.Count == 0)
        {
            error = playersResult.SeatLinesFound == 0
                ? "No 'Seat X: ... in chips' lines found"
                : $"Found {playersResult.SeatLinesFound} 'Seat ... in chips' lines but none parsed successfully";
            return false;
        }

        state.PlayersResult = playersResult;
        error = null;
        return true;
    }
}