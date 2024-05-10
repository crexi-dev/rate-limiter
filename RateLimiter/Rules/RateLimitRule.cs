using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public abstract class RateLimitRule
    {
        protected readonly IMemoryCache _memoryCache;

        public RateLimitRule(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public abstract string Name { get; }

        public abstract bool IsValid(RateLimitRuleModel rateLimitRuleModel, string token);
    }
}
