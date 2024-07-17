using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter;

public class DynamicDictionary<TKey, TValue>(IEnumerable<(Func<TKey, bool> predicate, Func<TKey, IEnumerable<TValue>> @delegate)> predicateDelegatePairs) where TKey : notnull
{
    private readonly Dictionary<TKey, IEnumerable<TValue>> _dictionary = [];
    public IEnumerable<TValue> Get(TKey key)
    {
        if (!_dictionary.ContainsKey(key))
            _dictionary.Add(key, predicateDelegatePairs.Where(x => x.predicate(key)).SelectMany(x => x.@delegate(key)).ToList());
        return _dictionary[key];
    }
}
