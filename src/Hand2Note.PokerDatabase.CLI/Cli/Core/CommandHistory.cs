namespace Hand2Note.PokerDatabase.CLI.Cli.Core;

public class CommandHistory(int maxHistorySize = 5)
{
    private readonly List<string> _history = [];
    private int _currentIndex = -1;

    public void Add(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return;

        _history.Remove(command);
        _history.Insert(0, command);

        if (_history.Count > maxHistorySize)
        {
            _history.RemoveAt(_history.Count - 1);
        }

        _currentIndex = -1;
    }

    public string? GetPrevious()
    {
        if (_history.Count == 0)
            return null;

        if (_currentIndex < _history.Count - 1)
        {
            _currentIndex++;
        }

        return _history[_currentIndex];
    }

    public string? GetNext()
    {
        if (_currentIndex <= 0)
        {
            _currentIndex = -1;
            return null;
        }

        _currentIndex--;
        return _history[_currentIndex];
    }

    public void ResetNavigation()
    {
        _currentIndex = -1;
    }
}