using Hand2Note.PokerDatabase.CLI.Commands.Core;
using Hand2Note.PokerDatabase.CLI.Storage;

namespace Hand2Note.PokerDatabase.CLI.Commands;

public class ShowPlayerHandsCommand(IHandRepository repository) : ICommand
{
    public string Name => "ShowPlayerHands";
    public string Description => "Shows the number of player hands and the last 10 hands";

    public CommandResult Execute(CommandArguments args)
    {
        var playerName = args.Get("PlayerName");
        if (string.IsNullOrEmpty(playerName))
        {
            return CommandResult.ErrorResult("PlayerName parameter is required");
        }

        var handsCount = repository.GetPlayerHandsCount(playerName);
        var lastHands = repository.GetPlayerLastHands(playerName, 10).ToList();

        var message = $"Player: {playerName}\nTotal hands: {handsCount}\n\nLast 10 hands:";
        
        var handsData = lastHands.Select(h =>
        {
            var player = h.Players.FirstOrDefault(p => string.Equals(p.Name, playerName, StringComparison.OrdinalIgnoreCase));
            var canonicalPlayerName = player?.Name ?? playerName;
            return new PlayerHandInfo
            {
                HandNumber = h.HandNumber,
                DealtCards = h.PlayerHands.GetValueOrDefault(canonicalPlayerName),
                StackSize = player?.StackSize ?? 0,
                Currency = player?.Currency
            };
        }).ToList();

        return CommandResult.SuccessResult(message, handsData);
    }
}