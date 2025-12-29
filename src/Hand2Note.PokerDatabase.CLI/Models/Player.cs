namespace Hand2Note.PokerDatabase.CLI.Models;

public class Player
{
    public required string Name { get; init; }
    public required decimal StackSize { get; init; }
    public string? Currency { get; init; }
}