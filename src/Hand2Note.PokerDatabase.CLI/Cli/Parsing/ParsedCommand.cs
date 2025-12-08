using Hand2Note.PokerDatabase.CLI.Commands.Core;

namespace Hand2Note.PokerDatabase.CLI.Cli.Parsing;

public class ParsedCommand
{
    public required string CommandName { get; init; }
    public required CommandArguments Arguments { get; init; }
}