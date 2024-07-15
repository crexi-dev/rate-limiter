using RateLimiter.Business.Rules;
using RateLimiter.Interfaces.Business.Rules;
using RateLimiter.Interfaces.Configuration;
using RateLimiter.Interfaces.DataAccess;
using RateLimiter.Interfaces.Factories;
using System;
using System.Collections.Generic;

namespace RateLimiter.Factories
{
    public class RateLimitRuleFactory : IRateLimitRuleFactory
    {
        private readonly IRateLimitRepository _repository;

        private readonly HashSet<string> _supportTypeNames = [
            nameof(CadenceRateLimitRule).ToLower(),
            nameof(RequestsPerWindowRateLimitRule).ToLower()
        ];

        public RateLimitRuleFactory(IRateLimitRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IRateLimitRule Create(IRateLimitRuleConfiguration configuration)
        {
            var key = configuration.Type.ToLower();

            if (key == nameof(CadenceRateLimitRule).ToLower())
            {
                return new CadenceRateLimitRule(_repository, configuration.Parameters);
            }

            if (key == nameof(RequestsPerWindowRateLimitRule).ToLower())
            {
                return new RequestsPerWindowRateLimitRule(_repository, configuration.Parameters);
            }

            throw new InvalidOperationException($"Invalid type: {configuration.Type}");
        }

        public bool SupportsType(string type)
        {
            return _supportTypeNames.Contains(type.ToLower());
        }
    }
}
