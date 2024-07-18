using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter;

/// <summary>
/// It's like a <see cref="Dictionary{TKey, TValue}"/>, except that rules are used to define what the dictionary will contain ahead of time!
/// Since multiple rules can affect multiple keys, all values returned are <see cref="IEnumerable{TValue}"/>.
/// Once a key is accessed, the value is cached. i.e The delegate used to return the value will not be run a second time, instead its result will be stored.
/// </summary>
/// <typeparam name="TKey">The type of key used to lookup values.</typeparam>
/// <typeparam name="TValue">The type of value to store.</typeparam>
/// <param name="predicateDelegatePairs">The rules that should be followed.</param>
public class DynamicDictionary<TKey, TValue>(IEnumerable<(Func<TKey, bool> predicate, Func<TKey, IEnumerable<TValue>> @delegate)> predicateDelegatePairs) where TKey : notnull
{
    private readonly Dictionary<TKey, IEnumerable<TValue>> _dictionary = [];
    
    /// <summary>
    /// Given a key, get the values that match the rules for that key.
    /// </summary>
    /// <param name="key">The key to lookup values.</param>
    /// <returns>All the values that match for the given key.</returns>
    public IEnumerable<TValue> Get(TKey key)
    {
        if (!_dictionary.ContainsKey(key))
            _dictionary.Add(key, predicateDelegatePairs.Where(predicateDelegatePair => predicateDelegatePair.predicate(key)).SelectMany(x => x.@delegate(key)).ToList());
        return _dictionary[key];
    }
}
