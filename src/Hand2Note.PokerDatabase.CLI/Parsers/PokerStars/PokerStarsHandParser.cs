using Hand2Note.PokerDatabase.CLI.Models;
using Hand2Note.PokerDatabase.CLI.Parsers.Core;
using Hand2Note.PokerDatabase.CLI.Parsers.PokerStars.Steps;

namespace Hand2Note.PokerDatabase.CLI.Parsers.PokerStars;

public class PokerStarsHandParser : IHandParser
{
    private static readonly List<IParseStep<ParseState>> Steps =
    [
        new HandNumberStep(),
        new HandDateStep(),
        new PlayersStep(),
        new DealtCardsStep()
    ];

    public Hand? Parse(string handText)
    {
        return ParseWithResult(handText).Hand;
    }

    public (Hand? Hand, ParseResult Result) ParseWithResult(string handText)
    {
        if (string.IsNullOrWhiteSpace(handText))
            return (null, ParseResult.ErrorResult("Empty or whitespace hand text"));

        var context = new HandParsingContext { HandText = handText };
        var state = new ParseState();

        foreach (var step in Steps)
        {
            if (!step.Execute(context, state, out var error))
            {
                return (null, ParseResult.ErrorResult(error ?? "Unknown parse error", state.HandNumber));
            }
        }

        if (!state.HandNumber.HasValue || !state.Date.HasValue || state.PlayersResult == null)
        {
            return (null, ParseResult.ErrorResult("Parser pipeline produced incomplete state", state.HandNumber));
        }

        var hand = new Hand
        {
            HandNumber = state.HandNumber.Value,
            Date = state.Date.Value,
            Players = state.PlayersResult.Players,
            PlayerHands = state.PlayerHands ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        };

        return (hand, ParseResult.SuccessResult(state.HandNumber.Value, state.PlayersResult.SeatLinesFound, state.PlayersResult.PlayersParsed));
    }
}