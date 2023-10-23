using RateLimiter.Contracts;
using RateLimiter.Data;
using System.Collections.Generic;
using Limiter = RateLimiter.Models.RateLimiter;

namespace RateLimiter
{
    public class DummyRateLimiterFacroty : IRateLimiterFactory
    {
        private IRateLimiterSettings _settings { get; init; }
        private IEnumerable<IClient> _clients { get; }

        public DummyRateLimiterFacroty(IRateLimiterSettings settings, IEnumerable<IClient> clients)
        {
            _settings = settings;
            _clients = clients;
        }

        public IRateLimiter GenerateRateLimitter()
        {
            var repo = new DummyRepository(_settings, _clients);
            var rateLimiter = new Limiter(repo);
            return rateLimiter;
        }
    }
}
