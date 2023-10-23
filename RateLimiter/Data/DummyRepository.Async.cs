using RateLimiter.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Data
{
    public partial class DummyRepository : IRepository
    {
        public Task SetRulesAsync(IEnumerable<IRequestLimitRule> rules)
        {
            _rules.UnionWith(rules);
            return Task.CompletedTask;
        }

        public Task<(IClient, IEnumerable<IRequestLimitRule>)> GetClientWithRulesAsync(Guid token, ResourcesType resource)
        {
            return Task.FromResult( GetClientWithRules(token, resource));
        }

        public Task AddClientAsync(IClient client)
        {
            _clients.Add(client.Token, client);
            return Task.CompletedTask;
        }

        public Task<(int count, DateTime lastCalltime)> GetClientLimitInfoAsync(Guid token, DateTime utcTime, TimeSpan period, ResourcesType resource)
        {
            return Task.FromResult(GetClientLimitInfo(token, utcTime, period, resource));
        }

        public Task IncremetAsync(Guid token, DateTime utcTime, ResourcesType resource)
        {
            Incremet(token, utcTime, resource);
            return Task.CompletedTask;
        }
    }
}
