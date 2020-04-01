using System;
using RateLimiter.RulesEngine.Library;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Library
{
    public interface IRuleCache
    {
        Rule this[string key] { get; set; }
    }
}