using Hand2Note.PokerDatabase.CLI.Parsers.PokerStars;

namespace Hand2Note.PokerDatabase.CLI.Parsers;

public class HandParserFactory
{
    private HandParserFactory()
    {
    }

    public static IHandParser CreateParser(string handText)
    {
        if (handText.Contains("PokerStars Hand #", StringComparison.Ordinal))
        {
            return new PokerStarsHandParser();
        }
        
        return new PokerStarsHandParser();
    }
}