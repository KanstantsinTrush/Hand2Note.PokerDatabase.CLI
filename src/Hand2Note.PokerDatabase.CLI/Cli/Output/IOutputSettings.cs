namespace Hand2Note.PokerDatabase.CLI.Cli.Output;

public interface IOutputSettings
{
    string DefaultCurrency { get; }
    string ColumnSeparator { get; }
    string MissingDataPlaceholder { get; }
    bool ShowTableHeaders { get; }
    bool ShowTableSeparators { get; }
    bool CurrencyBeforeAmount { get; }
}