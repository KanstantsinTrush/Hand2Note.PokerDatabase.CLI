using Hand2Note.PokerDatabase.CLI.Parsers.Core;
using Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Extractors;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Steps;

public sealed class HandNumberStep : IParseStep<ParseState>
{
    public bool Execute(HandParsingContext context, ParseState state, out string? error)
    {
        var extractor = new HandNumberExtractor();
        state.HandNumber = extractor.Extract(context);
        if (!state.HandNumber.HasValue)
        {
            error = "No 'Hand #' found in text";
            return false;
        }

        error = null;
        return true;
    }
}