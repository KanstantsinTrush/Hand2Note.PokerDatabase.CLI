using Hand2Note.PokerDatabase.CLI.Commands.Core;
using Hand2Note.PokerDatabase.CLI.Storage;

namespace Hand2Note.PokerDatabase.CLI.Commands;

public class ShowDeletedHandsCommand(IHandRepository repository) : ICommand
{
    public string Name => "ShowDeletedHands";
    public string Description => "Shows the numbers of deleted hands";

    public CommandResult Execute(CommandArguments args)
    {
        var deletedHands = repository.GetDeletedHandNumbers().ToList();

        if (deletedHands.Count == 0)
        {
            return CommandResult.SuccessResult("No deleted hands");
        }

        var message = $"Deleted hands ({deletedHands.Count}):";
        return CommandResult.SuccessResult(message, deletedHands);
    }
}