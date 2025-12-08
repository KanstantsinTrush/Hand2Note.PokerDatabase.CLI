using Hand2Note.PokerDatabase.CLI.Parsers.Core;
using Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Extractors;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Steps;

public sealed class HandDateStep : IParseStep<ParseState>
{
    public bool Execute(HandParsingContext context, ParseState state, out string? error)
    {
        var extractor = new HandDateExtractor();
        state.Date = extractor.Extract(context);
        if (!state.Date.HasValue)
        {
            error = "Could not parse hand date";
            return false;
        }

        error = null;
        return true;
    }
}