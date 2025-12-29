using Hand2Note.PokerDatabase.CLI.Commands.Core;
using Hand2Note.PokerDatabase.CLI.Storage;

namespace Hand2Note.PokerDatabase.CLI.Commands;

public class ShowStatsCommand(IHandRepository repository) : ICommand
{
    public string Name => "ShowStats";
    public string Description => "Shows the total number of hands and players in the database";

    public CommandResult Execute(CommandArguments args)
    {
        var totalHands = repository.GetTotalHandsCount();
        var uniquePlayers = repository.GetUniquePlayersCount();

        var message = $"Total hands: {totalHands}\nTotal players: {uniquePlayers}";
        return CommandResult.SuccessResult(message);
    }
}