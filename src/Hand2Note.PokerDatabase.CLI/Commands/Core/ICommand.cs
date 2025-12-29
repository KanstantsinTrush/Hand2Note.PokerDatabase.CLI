namespace Hand2Note.PokerDatabase.CLI.Commands.Core;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    CommandResult Execute(CommandArguments args);
}