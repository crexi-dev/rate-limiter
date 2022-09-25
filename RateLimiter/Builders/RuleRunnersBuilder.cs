using RateLimiter.Models;
using RateLimiter.RuleRunners;
using RateLimiter.Services;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Builders
{
    public class RuleRunnersBuilder : IRuleRunnersBuilder
	{
		private readonly ICacheService<RequestsPerTimeSpanCount> _requestsPerTimeSpanCountCacheService;
		private readonly ICacheService<TimeSpanSinceLastCall> _timeSpanSinceLastCallCountCacheService;
		public RuleRunnersBuilder(ICacheService<RequestsPerTimeSpanCount> requestsPerTimeSpanCountCacheService, 
			ICacheService<TimeSpanSinceLastCall> timeSpanSinceLastCallCountCacheService)
		{
			_requestsPerTimeSpanCountCacheService = requestsPerTimeSpanCountCacheService;
			_timeSpanSinceLastCallCountCacheService = timeSpanSinceLastCallCountCacheService;
		}
		public IEnumerable<IRuleRunner> GetRuleRunners(RateLimitRuleOptions options, ClientRequest request)
		{
			var runners = new List<IRuleRunner>();

			if ((!options?.Rules?.Any() ?? true) ||
				(!options?.Rules?.ContainsKey(request.Resource) ?? true))
			{
				return runners;
			}

			// get region based rules first
			if (options.Rules[request.Resource].RegionBasedRules?.Any() ?? false)
			{
				var regionRule = options.Rules[request.Resource].RegionBasedRules.Where(regionRule => regionRule.Region == request.Region).First();

				if (regionRule.RuleType == RuleType.RequestsPerTimeSpanRule && options.Rules[request.Resource].RequestsPerTimeSpanRule != null)
				{
					var rule = options.Rules[request.Resource].RequestsPerTimeSpanRule;
					runners.Add(new RequestsPerTimeSpanRuleRunner(rule, _requestsPerTimeSpanCountCacheService));
					return runners;
				}
				else if (regionRule.RuleType == RuleType.TimeSpanSinceLastCallRule && options.Rules[request.Resource].TimeSpanSinceLastCallRule != null)
				{
					var rule = options.Rules[request.Resource].TimeSpanSinceLastCallRule;
					runners.Add(new TimeSpanSinceLastCallRuleRunner(rule, _timeSpanSinceLastCallCountCacheService));
					return runners;
				}
			}

			if (options.Rules[request.Resource].RequestsPerTimeSpanRule != null)
			{
				var rule = options.Rules[request.Resource].RequestsPerTimeSpanRule;
				runners.Add(new RequestsPerTimeSpanRuleRunner(rule, _requestsPerTimeSpanCountCacheService));
			}

			if (options.Rules[request.Resource].TimeSpanSinceLastCallRule != null)
			{
				var rule = options.Rules[request.Resource].TimeSpanSinceLastCallRule;
				runners.Add(new TimeSpanSinceLastCallRuleRunner(rule, _timeSpanSinceLastCallCountCacheService));
			}

			return runners;
		}


	}
}
