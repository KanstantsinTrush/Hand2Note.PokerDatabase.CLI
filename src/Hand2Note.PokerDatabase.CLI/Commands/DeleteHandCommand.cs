using Hand2Note.PokerDatabase.CLI.Commands.Core;
using Hand2Note.PokerDatabase.CLI.Storage;

namespace Hand2Note.PokerDatabase.CLI.Commands;

public class DeleteHandCommand(IHandRepository repository) : ICommand
{
    public string Name => "DeleteHand";
    public string Description => "Deletes a hand with a specific number from the database";

    public CommandResult Execute(CommandArguments args)
    {
        var handNumberStr = args.Get("HandNumber");
        if (string.IsNullOrEmpty(handNumberStr))
        {
            return CommandResult.ErrorResult("HandNumber parameter is required");
        }

        if (!long.TryParse(handNumberStr, out var handNumber))
        {
            return CommandResult.ErrorResult($"Invalid HandNumber format: {handNumberStr}");
        }

        var hand = repository.GetHand(handNumber);
        if (hand == null)
        {
            return CommandResult.ErrorResult($"Hand #{handNumber} not found");
        }

        if (hand.Deleted)
        {
            return CommandResult.ErrorResult($"Hand #{handNumber} is already deleted");
        }

        repository.DeleteHand(handNumber);
        return CommandResult.SuccessResult($"Hand #{handNumber} has been deleted");
    }
}