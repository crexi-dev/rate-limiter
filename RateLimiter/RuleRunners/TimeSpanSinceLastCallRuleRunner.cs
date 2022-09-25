using RateLimiter.Models;
using RateLimiter.Services;
using System;
using System.Threading.Tasks;

namespace RateLimiter.RuleRunners
{
	public class TimeSpanSinceLastCallRuleRunner : IRuleRunner
	{
		private readonly TimeSpanSinceLastCallRule _rule;
		private readonly ICacheService<TimeSpanSinceLastCall> _cacheService;
		public TimeSpanSinceLastCallRuleRunner(TimeSpanSinceLastCallRule rule, ICacheService<TimeSpanSinceLastCall> cacheService)
		{
			_rule = rule;
			_cacheService = cacheService;
		}

		public async Task<RuleRunResult> RunAsync(ClientRequest request)
		{
			var cacheKey = request.TimeSpanSinceLastCallRuleCacheKey();
			if (!await _cacheService.ExistsAsync(cacheKey))
			{
				await _cacheService.AddAsync(cacheKey, new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow });

				return new RuleRunResult
				{
					IsSuccess = true
				};
			}

			var timeSpanSinceLastCall = await _cacheService.GetAsync(cacheKey);
			var dif = DateTimeOffset.UtcNow - timeSpanSinceLastCall.LastCallTime;
			if (dif > _rule.TimeSpan)
			{
				await _cacheService.RemoveAsync(cacheKey);
				await _cacheService.AddAsync(cacheKey, new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow });
				return new RuleRunResult
				{
					IsSuccess = true
				};
			}
			else
			{
				// update the last call time even for failures
				await _cacheService.RemoveAsync(cacheKey);
				await _cacheService.AddAsync(cacheKey, new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow });
				await Task.CompletedTask;
				return new RuleRunResult
				{
					IsSuccess = false,
					ErrorMessage = $"Time since last call is less than the allotted {_rule.TimeSpan.TotalMilliseconds}ms"
				};
			}
		}
	}
}
