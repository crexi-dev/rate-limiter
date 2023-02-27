using System;
using System.Collections.Concurrent;

public class RateLimiterOptions
{
    private readonly ConcurrentDictionary<string, Rules> _rules = new();

    public IRules Resource(string resourceName)
    {
        return _rules.GetOrAdd(resourceName, new Rules());
    }

    internal Rules? Get(string resource)
    {
        Rules? rules = null;
        _rules.TryGetValue(resource, out rules);
        return rules;
    }
}