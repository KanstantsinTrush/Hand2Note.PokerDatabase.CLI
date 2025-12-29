namespace Hand2Note.PokerDatabase.CLI.Parsers.Core;

public interface IParseStep<TState>
{
    bool Execute(HandParsingContext context, TState state, out string? error);
}