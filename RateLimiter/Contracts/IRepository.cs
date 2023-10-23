using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Contracts
{
    public interface IRepository
    {
        (IClient, IEnumerable<IRequestLimitRule>) GetClientWithRules(Guid token, ResourcesType resource);

        void SetRules(IEnumerable<IRequestLimitRule> rules);

        void AddClient(IClient client);

        (int count, DateTime lastCalltime) GetClientLimitInfo(Guid token, DateTime utcTime, TimeSpan period, ResourcesType resource);

        void Incremet(Guid token, DateTime utcTime, ResourcesType resource);


        Task<(IClient, IEnumerable<IRequestLimitRule>)> GetClientWithRulesAsync(Guid token, ResourcesType resource);

        Task SetRulesAsync(IEnumerable<IRequestLimitRule> rules);

        Task AddClientAsync(IClient client);

        Task<(int count, DateTime lastCalltime)> GetClientLimitInfoAsync(Guid token, DateTime utcTime, TimeSpan period, ResourcesType resource);
        Task IncremetAsync(Guid token, DateTime utcTime, ResourcesType resource);
    }
}
