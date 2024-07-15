using RateLimiter.Interfaces.Business.Rules;
using RateLimiter.Interfaces.Configuration;
using RateLimiter.Interfaces.Factories;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests.CustomRules
{
    public class CustomRuleFactory : IRateLimitRuleFactory
    {
        private readonly HashSet<string> _supportTypeNames = [
            nameof(BlacklistRule).ToLower()
        ];

        public IRateLimitRule Create(IRateLimitRuleConfiguration configuration)
        {
            var key = configuration.Type.ToLower();

            if (key == nameof(BlacklistRule).ToLower())
            {
                return new BlacklistRule(configuration.Parameters);
            }

            throw new InvalidOperationException($"Invalid type: {configuration.Type}");
        }

        public bool SupportsType(string type)
        {
            return _supportTypeNames.Contains(type.ToLower());
        }
    }
}
