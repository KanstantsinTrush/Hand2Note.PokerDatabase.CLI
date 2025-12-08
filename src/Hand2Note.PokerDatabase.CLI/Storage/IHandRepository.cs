using Hand2Note.PokerDatabase.CLI.Models;

namespace Hand2Note.PokerDatabase.CLI.Storage;

public interface IHandRepository
{
    void AddHand(Hand hand);
    Hand? GetHand(long handNumber);
    void DeleteHand(long handNumber);
    int GetTotalHandsCount();
    int GetUniquePlayersCount();
    int GetPlayerHandsCount(string playerName);
    IEnumerable<Hand> GetPlayerLastHands(string playerName, int count);
    IEnumerable<long> GetDeletedHandNumbers();
    IEnumerable<Hand> GetAllHands();
}