using Hand2Note.PokerDatabase.CLI.Models;

namespace Hand2Note.PokerDatabase.CLI.Parsers;

public interface IHandParser
{
    Hand? Parse(string handText);
}