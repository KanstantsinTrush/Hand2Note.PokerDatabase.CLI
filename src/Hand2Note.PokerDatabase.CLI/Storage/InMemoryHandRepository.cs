using Hand2Note.PokerDatabase.CLI.Models;

namespace Hand2Note.PokerDatabase.CLI.Storage;

public class InMemoryHandRepository : IHandRepository
{
    private readonly Dictionary<long, Hand> _hands = new();
    private readonly HashSet<string> _uniquePlayers = [];
    private readonly HashSet<long> _deletedHands = [];
    private readonly Dictionary<string, List<Hand>> _playerHands = new(StringComparer.OrdinalIgnoreCase);

    public void AddHand(Hand hand)
    {
        if (!_hands.TryAdd(hand.HandNumber, hand))
            return;

        foreach (var player in hand.Players)
        {
            _uniquePlayers.Add(player.Name);
            
            if (!_playerHands.TryGetValue(player.Name, out var value))
            {
                value = [];
                _playerHands[player.Name] = value;
            }

            value.Add(hand);
        }
    }

    public Hand? GetHand(long handNumber)
    {
        return _hands.GetValueOrDefault(handNumber);
    }

    public void DeleteHand(long handNumber)
    {
        if (_hands.TryGetValue(handNumber, out var hand))
        {
            hand.Deleted = true;
            _deletedHands.Add(handNumber);
        }
    }

    public int GetTotalHandsCount()
    {
        return _hands.Count;
    }

    public int GetUniquePlayersCount()
    {
        return _uniquePlayers.Count;
    }

    public int GetPlayerHandsCount(string playerName)
    {
        return _playerHands.TryGetValue(playerName, out var hands) ? hands.Count : 0;
    }

    public IEnumerable<Hand> GetPlayerLastHands(string playerName, int count)
    {
        if (!_playerHands.TryGetValue(playerName, out var hands))
            return [];

        return hands
            .Where(h => !h.Deleted && h.Players.Any(p => string.Equals(p.Name, playerName, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(h => h.Date)
            .ThenByDescending(h => h.HandNumber)
            .Take(count);
    }

    public IEnumerable<long> GetDeletedHandNumbers()
    {
        return _deletedHands.OrderBy(h => h);
    }

    public IEnumerable<Hand> GetAllHands()
    {
        return _hands.Values;
    }
}