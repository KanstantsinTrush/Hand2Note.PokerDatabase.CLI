namespace Hand2Note.PokerDatabase.CLI;

public class FileLoader
{
    private FileLoader()
    {
    }

    public static List<string> LoadHandHistories(string directoryPath)
    {
        var handHistories = new List<string>();

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
        }

        var files = Directory.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories);
        
        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            var hands = SplitIntoHands(content);
            handHistories.AddRange(hands);
        }

        return handHistories;
    }

    private static List<string> SplitIntoHands(string content)
    {
        var hands = new List<string>();
        var lines = content.Split('\n');
        var currentHand = new System.Text.StringBuilder();
        var handStarted = false;

        foreach (var line in lines)
        {
            if (line.Contains("PokerStars Hand #", StringComparison.Ordinal) ||
                line.Contains("Hand #", StringComparison.Ordinal))
            {
                if (handStarted && currentHand.Length > 0)
                {
                    hands.Add(currentHand.ToString());
                    currentHand.Clear();
                }
                handStarted = true;
            }

            if (handStarted)
            {
                currentHand.AppendLine(line);
            }
        }

        if (currentHand.Length > 0)
        {
            hands.Add(currentHand.ToString());
        }

        return hands;
    }
}