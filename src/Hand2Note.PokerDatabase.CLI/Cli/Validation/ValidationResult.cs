namespace Hand2Note.PokerDatabase.CLI.Cli.Validation;

public class ValidationResult
{
    public required bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }

    private ValidationResult()
    {
    }

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Error(string message)
    {
        return new ValidationResult { IsValid = false, ErrorMessage = message };
    }
}