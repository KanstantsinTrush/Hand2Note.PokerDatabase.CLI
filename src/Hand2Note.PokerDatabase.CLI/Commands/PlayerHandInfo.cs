namespace Hand2Note.PokerDatabase.CLI.Commands;

public class PlayerHandInfo
{
    public required long HandNumber { get; init; }
    public string? DealtCards { get; init; }
    public required decimal StackSize { get; init; }
    public string? Currency { get; init; }
}