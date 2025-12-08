using System;
using System.Collections.Generic;
using System.Text;
using Hand2Note.PokerDatabase.CLI.Commands.Core;

namespace Hand2Note.PokerDatabase.CLI.Cli.Parsing;

public class CommandParser
{
    public static ParsedCommand? Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var tokens = Tokenize(input);
        if (tokens.Count == 0)
            return null;

        var commandName = tokens[0];
        var args = new CommandArguments();

        for (var i = 1; i < tokens.Count; i++)
        {
            var token = tokens[i];
            
            if (token.StartsWith("--", StringComparison.Ordinal) || token.StartsWith("-", StringComparison.Ordinal))
            {
                var paramName = token.TrimStart('-');
                string? paramValue = null;

                if (i + 1 < tokens.Count && !IsFlag(tokens[i + 1]))
                {
                    paramValue = tokens[i + 1];
                    i++;
                }

                if (paramValue != null)
                {
                    args.Set(paramName, paramValue);
                }
            }
        }

        return new ParsedCommand
        {
            CommandName = commandName,
            Arguments = args
        };
    }

    private static List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        var currentToken = new System.Text.StringBuilder();
        var inQuotes = false;

        foreach (var ch in input)
        {
            if (ch == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(ch) && !inQuotes)
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }
            }
            else
            {
                currentToken.Append(ch);
            }
        }

        if (currentToken.Length > 0)
        {
            tokens.Add(currentToken.ToString());
        }

        return tokens;
    }

    private static bool IsFlag(string token)
    {
        if (token.StartsWith("--", StringComparison.Ordinal))
            return true;

        if (token.StartsWith("-", StringComparison.Ordinal) && token.Length > 1)
        {
            var secondChar = token[1];
            return !char.IsDigit(secondChar);
        }

        return false;
    }
}