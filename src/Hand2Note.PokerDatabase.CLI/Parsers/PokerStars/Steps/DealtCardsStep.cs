using Hand2Note.PokerDatabase.CLI.Parsers.Core;
using Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Extractors;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Steps;

public sealed class DealtCardsStep : IParseStep<ParseState>
{
    public bool Execute(HandParsingContext context, ParseState state, out string? error)
    {
        var extractor = new DealtCardsExtractor();
        state.PlayerHands = extractor.Extract(context);
        error = null;
        return true;
    }
}