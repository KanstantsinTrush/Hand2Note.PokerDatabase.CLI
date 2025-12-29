using Hand2Note.PokerDatabase.CLI.Cli.Core;
using Hand2Note.PokerDatabase.CLI.Commands.Core;
using Hand2Note.PokerDatabase.CLI.Parsers;
using Hand2Note.PokerDatabase.CLI.Parsers.PokerStars;
using Hand2Note.PokerDatabase.CLI.Storage;

namespace Hand2Note.PokerDatabase.CLI;

public class ApplicationRunner
{
    public void Run(string directoryPath)
    {
        var repository = new InMemoryHandRepository();
        var commandFactory = new CommandFactory(repository);
        var engine = new CommandLineEngine(commandFactory);

        Console.WriteLine($"Loading hand histories from: {directoryPath}");
        var handHistories = FileLoader.LoadHandHistories(directoryPath);
        Console.WriteLine($"Found {handHistories.Count} hand histories");

        Console.WriteLine("Parsing hand histories...");
        var parsedCount = 0;
        var errorCount = 0;
        var errorReasons = new Dictionary<string, int>();

        foreach (var handText in handHistories)
        {
            var parser = HandParserFactory.CreateParser(handText);
            
            if (parser is PokerStarsHandParser pokerStarsParser)
            {
                var (hand, result) = pokerStarsParser.ParseWithResult(handText);
                
                if (hand != null)
                {
                    repository.AddHand(hand);
                    parsedCount++;
                }
                else
                {
                    errorCount++;
                    var reason = result.ErrorReason ?? "Unknown error";
                    
                    errorReasons.TryAdd(reason, 0);
                    errorReasons[reason]++;
                }
            }
            else
            {
                var hand = parser.Parse(handText);
                if (hand != null)
                {
                    repository.AddHand(hand);
                    parsedCount++;
                }
                else
                {
                    errorCount++;
                    var reason = "Parser returned null";
                    errorReasons.TryAdd(reason, 0);
                    errorReasons[reason]++;
                }
            }
        }

        Console.WriteLine($"Parsed {parsedCount} hands successfully");
        if (errorCount > 0)
        {
            Console.WriteLine($"Failed to parse {errorCount} hands");
            Console.WriteLine();
            Console.WriteLine("Error reasons breakdown:");
            foreach (var (reason, count) in errorReasons.OrderByDescending(x => x.Value))
            {
                var percentage = (count * 100.0 / errorCount).ToString("F1");
                Console.WriteLine($"  {reason}: {count} ({percentage}%)");
            }
        }

        Console.WriteLine();

        engine.Run();
    }
}