namespace Hand2Note.PokerDatabase.CLI.Cli.Output;

public class DefaultOutputSettings : IOutputSettings
{
    public string DefaultCurrency => "$";
    public string ColumnSeparator => " | ";
    public string MissingDataPlaceholder => "N/A";
    public bool ShowTableHeaders => true;
    public bool ShowTableSeparators => true;
    public bool CurrencyBeforeAmount => true;

    private DefaultOutputSettings()
    {
    }

    public static DefaultOutputSettings Create()
    {
        return new DefaultOutputSettings();
    }
}