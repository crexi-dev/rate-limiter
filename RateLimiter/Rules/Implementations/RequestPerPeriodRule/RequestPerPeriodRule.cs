using RateLimiter.Core;
using RateLimiter.Rules.Base;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RateLimiter.Rules.Implementations.RequestPerPeriodRule
{
    public class RequestsPerPeriodRule(int? limit, TimeSpan? period, IRequestPerPeriodRuleRepository requestPerPeriodRuleRepository) : RateLimiterRuleBase
    {
        private readonly int? _limit = limit;
        private readonly TimeSpan? _period = period;
        private readonly IRequestPerPeriodRuleRepository _requestPerPeriodRuleRepository = requestPerPeriodRuleRepository;

        public override string Description { get { return "Requests per fixed period (e.g., 100 requests per minute)"; } }
        public override string Name { get { return "RequestsPerPeriodRule"; } }
        public override string ViolationMessage { get { return "Request limit exceeded."; } }

        public override Task<bool> IsRequestAllowedAsync(ClientRequestContext context)
        {
            var key = base.GenerateRequestKey(context);
            var now = DateTime.UtcNow;
            var counter = 0;

            var entry = _requestPerPeriodRuleRepository.GetOrAdd(key, now, 0);

            if (now - entry.start > _period)
                entry = (now, 0);
            else
                counter = entry.count + 1;

            if (counter >= _limit)
                return Task.FromResult(false);

            _requestPerPeriodRuleRepository.Update(key, entry.start, counter);

            return Task.FromResult(true);
        }

    }
}
