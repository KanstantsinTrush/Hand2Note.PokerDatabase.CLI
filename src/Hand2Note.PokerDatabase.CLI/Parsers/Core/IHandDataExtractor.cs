namespace Hand2Note.PokerDatabase.CLI.Parsers.Core;

public interface IHandDataExtractor<T>
{
    T Extract(HandParsingContext context);
}