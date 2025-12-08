using System;
using System.Collections.Generic;

namespace Hand2Note.PokerDatabase.CLI.Commands.Core;

public class CommandArguments
{
    private readonly Dictionary<string, string> _parameters = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _aliases = new(StringComparer.OrdinalIgnoreCase);

    public CommandArguments()
    {
        RegisterAlias("n", "HandNumber");
        RegisterAlias("p", "PlayerName");
    }

    public void Set(string key, string value)
    {
        var normalizedKey = NormalizeKey(key);
        
        if (_aliases.TryGetValue(normalizedKey, out var fullName))
        {
            _parameters[fullName] = value;
        }
        else
        {
            _parameters[normalizedKey] = value;
        }
    }

    public string? Get(string key)
    {
        var normalizedKey = NormalizeKey(key);
        
        if (_parameters.TryGetValue(normalizedKey, out var value))
            return value;

        if (_aliases.TryGetValue(normalizedKey, out var fullName))
        {
            if (_parameters.TryGetValue(fullName, out var aliasValue))
                return aliasValue;
        }

        return null;
    }

    public bool Has(string key)
    {
        var normalizedKey = NormalizeKey(key);
        if (_parameters.ContainsKey(normalizedKey))
            return true;

        if (_aliases.TryGetValue(normalizedKey, out var aliasKey))
            return _parameters.ContainsKey(aliasKey);

        return false;
    }

    public void RegisterAlias(string alias, string fullName)
    {
        _aliases[alias] = fullName;
    }

    private string NormalizeKey(string key)
    {
        return key.TrimStart('-');
    }
}