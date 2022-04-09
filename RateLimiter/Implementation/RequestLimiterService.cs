using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Implementation
{
    public class RequestLimiterService : IRequestLimiterService
    {
        private readonly ICacheService _cacheService;

        public Dictionary<string, IEnumerable<IRule>> Rules = new Dictionary<string, IEnumerable<IRule>>()
        {
          {"api/getFromUS" , new List<IRule> { new LimitByDateRangeRule() { IntervalMS = 100, MaxReqeustCount = 10 } } },
          {"api/getFromEU" , new List<IRule> { new LimitByIntervalRule() { IntervalMS = 10 } } },
          {"api/getFromGlobal" , new List<IRule> { new LimitByDateRangeRule() { IntervalMS = 100, MaxReqeustCount = 10 }, new LimitByIntervalRule() { IntervalMS = 10 } } },
        };

        public RequestLimiterService(ICacheService cacheService)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public bool MakeRequest(string token, string apiKey)
        {
            IEnumerable<IRule> rules;
            Rules.TryGetValue(apiKey, out rules);

            if (rules != null && rules.All(s => s.IsRuleValid(_cacheService, token)))
            {
                return true;
            }
            return false;
        }
    }
}
