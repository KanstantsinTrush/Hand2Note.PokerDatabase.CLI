namespace Hand2Note.PokerDatabase.CLI.Models;

public class Hand
{
    public required long HandNumber { get; init; }
    public required DateTime Date { get; init; }
    public required List<Player> Players { get; init; }
    public required Dictionary<string, string> PlayerHands { get; init; }
    public bool Deleted { get; set; }
}