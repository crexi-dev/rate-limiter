using System.Collections.Concurrent;
using System.Collections.Generic;
using RateLimiter.Rules;

namespace RateLimiter;

public class RateLimiter
{
    private readonly Dictionary<string, List<IRule>> _rules;
    private static readonly ConcurrentDictionary<string, UserRegInfo> _requestStorage = new();

    public RateLimiter(Dictionary<string, List<IRule>>? rules = null)
    {
        _rules = rules ?? new Dictionary<string, List<IRule>>();
    }

    public bool TryRequest(string token)
    {
        // looking for registrations
        UserRegInfo userRegInfo;
        if (!_requestStorage.TryGetValue(token, out userRegInfo))
        {
            // if it's new - create new instance
            userRegInfo = new UserRegInfo
            {
                Token = token,
                ConnectionsCount = 0, // ConnectionsCount - 0 for new tokens
            };
        }
        
        // cheking for rules
        if (_rules.TryGetValue(token, out var rules))
        {
            foreach (var rule in rules)
            {
                if (!rule.CheckAndUpdate(userRegInfo))
                {
                    return false;
                }
            }
        }

        // updating registration storage
        _requestStorage.TryAdd(userRegInfo.Token, userRegInfo);

        return true;
    }
}