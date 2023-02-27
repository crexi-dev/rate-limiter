using System.Collections.Generic;

internal class Rules : IRules
{
    private readonly List<IRule> _rules = new();
    public List<IRule> GetRules() => _rules;

    public IRules Add(IRule rule)
    {
        _rules.Add(rule);
        return this;
    }
}