using System;
using System.Linq;
using Hand2Note.PokerDatabase.CLI.Cli.Output;
using Hand2Note.PokerDatabase.CLI.Cli.Parsing;
using Hand2Note.PokerDatabase.CLI.Cli.Validation;
using Hand2Note.PokerDatabase.CLI.Commands.Core;

namespace Hand2Note.PokerDatabase.CLI.Cli.Core;

public class CommandLineEngine
{
    private readonly CommandFactory _commandFactory;
    private readonly OutputFormatter _formatter;
    private readonly CommandHistory _history = new(maxHistorySize: 5);
    private readonly ConsoleInputReader _inputReader;

    public CommandLineEngine(CommandFactory commandFactory, IOutputSettings? outputSettings = null)
    {
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        var settings = outputSettings ?? DefaultOutputSettings.Create();
        _formatter = new OutputFormatter(settings);
        _inputReader = new ConsoleInputReader(_history);
    }

    public void Run()
    {
        Console.WriteLine("Poker Database CLI. Type 'exit' or 'quit' to exit.");
        Console.WriteLine("Use Up/Down arrows to navigate command history.");
        Console.WriteLine();

        while (true)
        {
            var input = _inputReader.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            try
            {
                ProcessCommand(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    private void ProcessCommand(string input)
    {
        var parsedCommand = CommandParser.Parse(input);
        if (parsedCommand == null)
        {
            Console.WriteLine("Error: Invalid command format");
            return;
        }

        var command = _commandFactory.GetCommand(parsedCommand.CommandName);
        if (command == null)
        {
            Console.WriteLine($"Error: Unknown command '{parsedCommand.CommandName}'");
            Console.WriteLine("Available commands:");
            foreach (var cmd in _commandFactory.GetAllCommands().Distinct())
            {
                Console.WriteLine($"  {cmd.Name} - {cmd.Description}");
            }
            return;
        }

        var validationResult = CommandValidator.Validate(command, parsedCommand.Arguments);
        if (!validationResult.IsValid)
        {
            Console.WriteLine($"Error: {validationResult.ErrorMessage}");
            return;
        }

        var result = command.Execute(parsedCommand.Arguments);

        _formatter.Format(result);
    }
}