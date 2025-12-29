namespace Hand2Note.PokerDatabase.CLI.Commands.Core;

public class CommandResult
{
    public required bool Success { get; init; }
    public required string Message { get; init; }
    public object? Data { get; init; }

    private CommandResult()
    {
    }

    public static CommandResult SuccessResult(string message, object? data = null)
    {
        return new CommandResult { Success = true, Message = message, Data = data };
    }

    public static CommandResult ErrorResult(string message)
    {
        return new CommandResult { Success = false, Message = message };
    }
}