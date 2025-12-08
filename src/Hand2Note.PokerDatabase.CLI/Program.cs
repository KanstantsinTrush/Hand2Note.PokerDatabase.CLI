namespace Hand2Note.PokerDatabase.CLI;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: Hand2Note.PokerDatabase.CLI <directory_path>");
            Console.WriteLine("Example: Hand2Note.PokerDatabase.CLI ./history");
            return;
        }

        var directoryPath = args[0];

        try
        {
            var runner = new ApplicationRunner();
            runner.Run(directoryPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
}