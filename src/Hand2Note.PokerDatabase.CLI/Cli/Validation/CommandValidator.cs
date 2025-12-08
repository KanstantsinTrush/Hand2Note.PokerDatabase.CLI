using Hand2Note.PokerDatabase.CLI.Commands;
using Hand2Note.PokerDatabase.CLI.Commands.Core;

namespace Hand2Note.PokerDatabase.CLI.Cli.Validation;

public class CommandValidator
{
    public static ValidationResult Validate(ICommand command, CommandArguments args)
    {
        if (command is ShowPlayerHandsCommand)
        {
            if (!args.Has("PlayerName"))
            {
                return ValidationResult.Error("PlayerName parameter is required");
            }
        }
        else if (command is DeleteHandCommand)
        {
            if (!args.Has("HandNumber"))
            {
                return ValidationResult.Error("HandNumber parameter is required");
            }

            var handNumberStr = args.Get("HandNumber");
            if (!long.TryParse(handNumberStr, out _))
            {
                return ValidationResult.Error($"Invalid HandNumber format: {handNumberStr}");
            }
        }

        return ValidationResult.Success();
    }
}