namespace Hand2Note.PokerDatabase.CLI.Cli.Core;

public class ConsoleInputReader(CommandHistory history)
{
    private readonly CommandHistory _history = history ?? throw new ArgumentNullException(nameof(history));
    private const string Prompt = "> ";

    public string? ReadLine()
    {
        Console.Write(Prompt);
        
        var input = new List<char>();
        string? currentHistoryItem = null;

        while (true)
        {
            var keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                var result = new string(input.ToArray());
                if (!string.IsNullOrWhiteSpace(result))
                {
                    _history.Add(result);
                }
                _history.ResetNavigation();
                return result;
            }

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                _history.ResetNavigation();
                return null;
            }

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                var previousCommand = _history.GetPrevious();
                if (previousCommand != null)
                {
                    ClearInputLine(input.Count);
                    input = previousCommand.ToList();
                    currentHistoryItem = previousCommand;
                    Console.Write(previousCommand);
                }
                continue;
            }

            if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                var nextCommand = _history.GetNext();
                ClearInputLine(input.Count);
                if (nextCommand != null)
                {
                    input = nextCommand.ToList();
                    currentHistoryItem = nextCommand;
                    Console.Write(nextCommand);
                }
                else
                {
                    input.Clear();
                    currentHistoryItem = null;
                }
                continue;
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (input.Count > 0)
                {
                    input.RemoveAt(input.Count - 1);
                    ClearInputLine(input.Count + 1);
                    Console.Write(new string(input.ToArray()));
                    currentHistoryItem = null;
                }
                continue;
            }

            if (!char.IsControl(keyInfo.KeyChar))
            {
                if (currentHistoryItem != null && new string(input.ToArray()) != currentHistoryItem)
                {
                    _history.ResetNavigation();
                    currentHistoryItem = null;
                }
                
                input.Add(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
        }
    }

    private static void ClearInputLine(int inputLength)
    {
        var currentLeft = Console.CursorLeft;
        var currentTop = Console.CursorTop;
        
        Console.SetCursorPosition(Prompt.Length, currentTop);
        
        var clearLength = Math.Max(inputLength, currentLeft - Prompt.Length);
        Console.Write(new string(' ', clearLength));
        
        Console.SetCursorPosition(Prompt.Length, currentTop);
    }
}