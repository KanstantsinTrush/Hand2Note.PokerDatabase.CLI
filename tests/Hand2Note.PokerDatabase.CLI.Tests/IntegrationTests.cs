using Hand2Note.PokerDatabase.CLI.Cli.Core;
using Hand2Note.PokerDatabase.CLI.Cli.Output;
using Hand2Note.PokerDatabase.CLI.Cli.Parsing;
using Hand2Note.PokerDatabase.CLI.Commands;
using Hand2Note.PokerDatabase.CLI.Commands.Core;
using Hand2Note.PokerDatabase.CLI.Parsers;
using Hand2Note.PokerDatabase.CLI.Parsers.PokerStars;
using Hand2Note.PokerDatabase.CLI.Storage;

namespace Hand2Note.PokerDatabase.CLI.Tests;

public class IntegrationTests
{
    private readonly string _testDataPath;

    public IntegrationTests()
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        _testDataPath = Path.Combine(projectRoot, "test-data");
    }

    [Fact]
    public void LoadAndParseRealHandHistories()
    {
        // Arrange
        var repository = new InMemoryHandRepository();

        // Act
        var handHistories = FileLoader.LoadHandHistories(_testDataPath);
        Assert.NotEmpty(handHistories);

        int parsedCount = 0;
        foreach (var handText in handHistories)
        {
            var parser = HandParserFactory.CreateParser(handText);
            var hand = parser.Parse(handText);
            if (hand != null)
            {
                repository.AddHand(hand);
                parsedCount++;
            }
        }

        // Assert
        Assert.True(parsedCount > 0, "Should parse at least one hand");
        Assert.True(repository.GetTotalHandsCount() > 0, "Repository should contain hands");
        Assert.True(repository.GetUniquePlayersCount() > 0, "Repository should contain players");
    }

    [Fact]
    public void ShowStatsCommand_WorksWithRealData()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var command = new ShowStatsCommand(repository);

        // Act
        var result = command.Execute(new CommandArguments());

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Total hands:", result.Message);
        Assert.Contains("Total players:", result.Message);
        
        var totalHands = repository.GetTotalHandsCount();
        var totalPlayers = repository.GetUniquePlayersCount();
        Assert.True(totalHands > 0);
        Assert.True(totalPlayers > 0);
    }

    [Fact]
    public void ShowPlayerHandsCommand_WorksWithRealData()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var command = new ShowPlayerHandsCommand(repository);
        
        var allHands = GetAllHandsFromRepository(repository);
        Assert.NotEmpty(allHands);
        
        var playerName = allHands
            .SelectMany(h => h.Players)
            .Select(p => p.Name)
            .FirstOrDefault(name => !string.IsNullOrEmpty(name));
        
        Assert.NotNull(playerName);

        var args = new CommandArguments();
        args.Set("PlayerName", playerName);

        // Act
        var result = command.Execute(args);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Player:", result.Message);
        Assert.Contains("Total hands:", result.Message);
        Assert.NotNull(result.Data);
        
        var handsCount = repository.GetPlayerHandsCount(playerName);
        Assert.True(handsCount > 0);
    }

    [Fact]
    public void DeleteHandCommand_WorksWithRealData()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var command = new DeleteHandCommand(repository);
        
        var allHands = GetAllHandsFromRepository(repository);
        Assert.NotEmpty(allHands);
        
        var handNumber = allHands.First().HandNumber;
        Assert.True(handNumber > 0);

        var args = new CommandArguments();
        args.Set("HandNumber", handNumber.ToString());

        // Act
        var result = command.Execute(args);

        // Assert
        Assert.True(result.Success);
        Assert.Contains($"Hand #{handNumber} has been deleted", result.Message);
        
        var deletedHand = repository.GetHand(handNumber);
        Assert.NotNull(deletedHand);
        Assert.True(deletedHand.Deleted);
    }

    [Fact]
    public void DeleteHandCommand_WithShortParameter_Works()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var command = new DeleteHandCommand(repository);
        
        var allHands = GetAllHandsFromRepository(repository);
        var handNumber = allHands.Skip(1).First().HandNumber;

        var args = new CommandArguments();
        args.Set("n", handNumber.ToString());

        // Act
        var result = command.Execute(args);

        // Assert
        Assert.True(result.Success);
        Assert.Contains($"Hand #{handNumber} has been deleted", result.Message);
    }

    [Fact]
    public void ShowDeletedHandsCommand_WorksWithRealData()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var deleteCommand = new DeleteHandCommand(repository);
        var showDeletedCommand = new ShowDeletedHandsCommand(repository);
        
        var allHands = GetAllHandsFromRepository(repository);
        var handsToDelete = allHands.Take(3).ToList();
        
        foreach (var hand in handsToDelete)
        {
            var args = new CommandArguments();
            args.Set("HandNumber", hand.HandNumber.ToString());
            deleteCommand.Execute(args);
        }

        // Act
        var result = showDeletedCommand.Execute(new CommandArguments());

        // Assert
        Assert.True(result.Success);
        var deletedHands = repository.GetDeletedHandNumbers().ToList();
        Assert.True(deletedHands.Count >= 3);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public void ShowPlayerHandsCommand_ShowsCorrectFormat()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var command = new ShowPlayerHandsCommand(repository);
        
        var allHands = GetAllHandsFromRepository(repository);
        var playerName = allHands
            .Where(h => h.PlayerHands.ContainsKey(h.Players.FirstOrDefault()?.Name ?? ""))
            .SelectMany(h => h.Players)
            .Where(p => !string.IsNullOrEmpty(p.Name))
            .Select(p => p.Name)
            .FirstOrDefault();
        
        Assert.NotNull(playerName);

        var args = new CommandArguments();
        args.Set("PlayerName", playerName);

        // Act
        var result = command.Execute(args);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        
        if (result.Data is IEnumerable<PlayerHandInfo> handsData)
        {
            var handsList = handsData.ToList();
            Assert.True(handsList.Count <= 10, "Should return at most 10 hands");
            
            foreach (var hand in handsList)
            {
                Assert.True(hand.HandNumber > 0);
                Assert.NotNull(hand.DealtCards);
            }
        }
    }

    [Fact]
    public void CommandParser_ParsesCommandsCorrectly()
    {
        // Arrange

        // Act & Assert
        var result1 = CommandParser.Parse("ShowStats");
        Assert.NotNull(result1);
        Assert.Equal("ShowStats", result1.CommandName);

        var result2 = CommandParser.Parse("ShowPlayerHands --PlayerName angrypaca");
        Assert.NotNull(result2);
        Assert.Equal("ShowPlayerHands", result2.CommandName);
        Assert.Equal("angrypaca", result2.Arguments.Get("PlayerName"));

        var result3 = CommandParser.Parse("DeleteHand -n 123456");
        Assert.NotNull(result3);
        Assert.Equal("DeleteHand", result3.CommandName);
        Assert.Equal("123456", result3.Arguments.Get("HandNumber"));
    }

    [Fact]
    public void CommandLineEngine_ProcessesCommands()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var commandFactory = new CommandFactory(repository);

        var statsCommand = commandFactory.GetCommand("ShowStats");
        Assert.NotNull(statsCommand);
        
        var statsAlias = commandFactory.GetCommand("Stats");
        Assert.NotNull(statsAlias);
        Assert.Same(statsCommand, statsAlias);
    }

    [Fact]
    public void DeleteHandCommand_WithNonExistentHand_ReturnsError()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var command = new DeleteHandCommand(repository);
        var args = new CommandArguments();
        args.Set("HandNumber", "999999999999");

        // Act
        var result = command.Execute(args);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message);
    }

    [Fact]
    public void DeleteHandCommand_WithInvalidFormat_ReturnsError()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var command = new DeleteHandCommand(repository);
        var args = new CommandArguments();
        args.Set("HandNumber", "invalid");

        // Act
        var result = command.Execute(args);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Invalid HandNumber format", result.Message);
    }

    [Fact]
    public void ShowPlayerHandsCommand_WithNonExistentPlayer_ReturnsZeroHands()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var command = new ShowPlayerHandsCommand(repository);
        var args = new CommandArguments();
        args.Set("PlayerName", "NonExistentPlayer12345");

        // Act
        var result = command.Execute(args);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Total hands: 0", result.Message);
    }

    [Fact]
    public void ShowPlayerHandsCommand_WithShortParameter_Works()
    {
        // Arrange
        var repository = LoadRepositoryWithRealData();
        var command = new ShowPlayerHandsCommand(repository);
        
        var allHands = GetAllHandsFromRepository(repository);
        var playerName = allHands
            .SelectMany(h => h.Players)
            .Select(p => p.Name)
            .FirstOrDefault(name => !string.IsNullOrEmpty(name));
        
        Assert.NotNull(playerName);

        var args = new CommandArguments();
        args.Set("p", playerName);

        // Act
        var result = command.Execute(args);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Player:", result.Message);
    }

    [Fact]
    public void ShowDeletedHandsCommand_WithNoDeletedHands_ReturnsEmpty()
    {
        // Arrange
        var repository = new InMemoryHandRepository();
        var handHistories = FileLoader.LoadHandHistories(_testDataPath);

        var parser = HandParserFactory.CreateParser(handHistories.First());
        var hand = parser.Parse(handHistories.First());
        if (hand != null)
        {
            repository.AddHand(hand);
        }

        var command = new ShowDeletedHandsCommand(repository);

        // Act
        var result = command.Execute(new CommandArguments());

        // Assert
        Assert.True(result.Success);
        Assert.Contains("No deleted hands", result.Message);
    }

    [Fact]
    public void Parser_ParsesHandWithMultiplePlayers()
    {
        // Arrange
        var parser = new PokerStarsHandParser();
        var handText = FileLoader.LoadHandHistories(_testDataPath).First();

        // Act
        var hand = parser.Parse(handText);

        // Assert
        Assert.NotNull(hand);
        Assert.True(hand.HandNumber > 0);
        Assert.NotNull(hand.Players);
        Assert.True(hand.Players.Count > 0);
    }

    [Fact]
    public void Parser_ParsesDealtCards()
    {
        // Arrange
        var parser = new PokerStarsHandParser();
        var handText = FileLoader.LoadHandHistories(_testDataPath)
            .First(h => h.Contains("Dealt to", StringComparison.Ordinal));

        // Act
        var hand = parser.Parse(handText);

        // Assert
        Assert.NotNull(hand);
        Assert.NotNull(hand.PlayerHands);
        Assert.True(hand.PlayerHands.Count > 0, "Should parse at least one player's cards");
        
        foreach (var playerHand in hand.PlayerHands)
        {
            Assert.False(string.IsNullOrEmpty(playerHand.Value), $"Cards for {playerHand.Key} should not be empty");
        }
    }

    private IHandRepository LoadRepositoryWithRealData()
    {
        var repository = new InMemoryHandRepository();
        var handHistories = FileLoader.LoadHandHistories(_testDataPath);

        foreach (var handText in handHistories)
        {
            var parser = HandParserFactory.CreateParser(handText);
            var hand = parser.Parse(handText);
            if (hand != null)
            {
                repository.AddHand(hand);
            }
        }

        return repository;
    }

    private List<Models.Hand> GetAllHandsFromRepository(IHandRepository repository)
    {
        return repository.GetAllHands().ToList();
    }

    [Fact]
    public void ParseSpecificHand_93404843251()
    {
        // Arrange
        var filePath = Path.Combine(_testDataPath, "2013", "02", "03", "HH20130202 Libussa IV - $0.10-$0.25 - USD No Limit Hold'em.txt");
        Assert.True(File.Exists(filePath), $"Test file not found: {filePath}");

        var content = File.ReadAllText(filePath);
        
        var handStart = content.IndexOf("Hand #93404843251:", StringComparison.Ordinal);
        Assert.True(handStart >= 0, "Hand #93404843251 not found in file");
        
        var handEnd = content.IndexOf("Hand #", handStart + 1, StringComparison.Ordinal);
        var handText = handEnd >= 0 
            ? content.Substring(handStart, handEnd - handStart)
            : content.Substring(handStart);

        var handsFromLoader = FileLoader.LoadHandHistories(Path.Combine(_testDataPath, "2013", "02", "03"));
        var loadedHand = handsFromLoader.FirstOrDefault(h => h.Contains("Hand #93404843251:"));
        if (loadedHand != null)
        {
            handText = loadedHand;
        }

        // Act
        var parser = HandParserFactory.CreateParser(handText);
        if (parser is PokerStarsHandParser pokerStarsParser)
        {
            var (hand, result) = pokerStarsParser.ParseWithResult(handText);
            
            if (hand == null)
            {
                var errorInfo = $"Parse failed. Error: {result.ErrorReason}, HandNumber: {result.HandNumber}, " +
                               $"PlayersFound: {result.PlayersFound}, PlayersParsed: {result.PlayersParsed}";
                Assert.Fail(errorInfo);
            }
            
            Assert.NotNull(hand);
            Assert.True(result.Success);
            Assert.Equal(93404843251L, hand.HandNumber);
            Assert.NotNull(hand.Players);
            Assert.Equal(6, hand.Players.Count);
            Assert.Equal(6, result.PlayersParsed);
            
            var angrypaca = hand.Players.FirstOrDefault(p => p.Name == "angrypaca");
            Assert.NotNull(angrypaca);
            Assert.Equal(31.70m, angrypaca.StackSize);
            Assert.Equal("$", angrypaca.Currency);
            
            Assert.NotNull(hand.PlayerHands);
            Assert.True(hand.PlayerHands.ContainsKey("angrypaca"));
            Assert.Equal("3h Ks", hand.PlayerHands["angrypaca"]);
        }
        else
        {
            var hand = parser.Parse(handText);
            Assert.NotNull(hand);
            Assert.Equal(93404843251L, hand.HandNumber);
        }
    }
}