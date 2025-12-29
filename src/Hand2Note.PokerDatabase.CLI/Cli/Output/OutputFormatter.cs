using System;
using System.Collections.Generic;
using System.Linq;
using Hand2Note.PokerDatabase.CLI.Commands;
using Hand2Note.PokerDatabase.CLI.Commands.Core;

namespace Hand2Note.PokerDatabase.CLI.Cli.Output;

public class OutputFormatter(IOutputSettings settings)
{
    private readonly IOutputSettings _settings = settings ?? throw new ArgumentNullException(nameof(settings));

    public void Format(CommandResult result)
    {
        if (!result.Success)
        {
            Console.WriteLine($"Error: {result.Message}");
            return;
        }

        Console.WriteLine(result.Message);

        if (result.Data == null)
            return;

        if (result.Data is IEnumerable<PlayerHandInfo> handsData)
        {
            FormatPlayerHands(handsData);
        }
        else if (result.Data is IEnumerable<long> deletedHands)
        {
            FormatDeletedHands(deletedHands);
        }
    }

    private void FormatPlayerHands(IEnumerable<PlayerHandInfo> handsData)
    {
        var handsList = handsData.ToList();
        if (handsList.Count == 0)
            return;

        Console.WriteLine();

        if (_settings.ShowTableHeaders)
        {
            Console.WriteLine($"Hand Number{_settings.ColumnSeparator}Dealt Cards{_settings.ColumnSeparator}Stack Size");
        }

        if (_settings.ShowTableSeparators)
        {
            var separatorLength = handsList.Max(h => 
                $"{h.HandNumber}{_settings.ColumnSeparator}{h.DealtCards ?? _settings.MissingDataPlaceholder}{_settings.ColumnSeparator}{FormatStackSize(h.StackSize, h.Currency)}".Length);
            Console.WriteLine(new string('-', Math.Max(50, separatorLength)));
        }

        foreach (var hand in handsList)
        {
            var cards = hand.DealtCards ?? _settings.MissingDataPlaceholder;
            var stackSize = FormatStackSize(hand.StackSize, hand.Currency);
            Console.WriteLine($"{hand.HandNumber}{_settings.ColumnSeparator}{cards}{_settings.ColumnSeparator}{stackSize}");
        }
    }

    private string FormatStackSize(decimal stackSize, string? currency)
    {
        var actualCurrency = currency ?? _settings.DefaultCurrency;
        
        if (_settings.CurrencyBeforeAmount)
        {
            return $"{actualCurrency}{stackSize}";
        }
        else
        {
            return $"{stackSize} {actualCurrency}";
        }
    }

    private void FormatDeletedHands(IEnumerable<long> deletedHands)
    {
        var deletedList = deletedHands.ToList();
        if (deletedList.Count > 0)
        {
            Console.WriteLine(string.Join(", ", deletedList));
        }
    }
}