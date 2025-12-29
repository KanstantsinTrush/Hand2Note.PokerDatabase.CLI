using Hand2Note.PokerDatabase.CLI.Storage;

namespace Hand2Note.PokerDatabase.CLI.Commands.Core;

public class CommandFactory
{
    private readonly Dictionary<string, ICommand> _commands = new(StringComparer.OrdinalIgnoreCase);

    public CommandFactory(IHandRepository repository)
    {
        RegisterCommand(new ShowStatsCommand(repository));
        RegisterCommand(new ShowPlayerHandsCommand(repository));
        RegisterCommand(new DeleteHandCommand(repository));
        RegisterCommand(new ShowDeletedHandsCommand(repository));

        RegisterAlias("Stats", "ShowStats");
        RegisterAlias("PlayerHands", "ShowPlayerHands");
        RegisterAlias("DeletedHands", "ShowDeletedHands");
    }

    public ICommand? GetCommand(string commandName)
    {
        return _commands.TryGetValue(commandName, out var command) ? command : null;
    }

    public IEnumerable<ICommand> GetAllCommands()
    {
        return _commands.Values.Distinct();
    }

    private void RegisterCommand(ICommand command)
    {
        _commands[command.Name] = command;
    }

    private void RegisterAlias(string alias, string commandName)
    {
        if (_commands.TryGetValue(commandName, out var command))
        {
            _commands[alias] = command;
        }
    }
}