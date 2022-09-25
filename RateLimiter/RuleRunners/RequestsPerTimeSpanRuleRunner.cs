using RateLimiter.Models;
using RateLimiter.Services;
using System;
using System.Threading.Tasks;

namespace RateLimiter.RuleRunners
{
    public class RequestsPerTimeSpanRuleRunner : IRuleRunner
	{
		private readonly RequestsPerTimeSpanRule _rule;
		private readonly ICacheService<RequestsPerTimeSpanCount> _cacheService;
		public RequestsPerTimeSpanRuleRunner(RequestsPerTimeSpanRule rule, ICacheService<RequestsPerTimeSpanCount> cacheService)
		{
			_rule = rule;
			_cacheService = cacheService;
		}

		public async Task<RuleRunResult> RunAsync(ClientRequest request)
		{
			var cacheKey = request.RequestsPerTimeSpanRuleCacheKey();
			if (!await _cacheService.ExistsAsync(cacheKey))
			{
				await _cacheService.AddAsync(cacheKey, new RequestsPerTimeSpanCount { StartTime = DateTimeOffset.UtcNow, Count = 1 });

				return new RuleRunResult
				{
					IsSuccess = true
				};
			}

			var count = await _cacheService.GetAsync(cacheKey);
			var dif = DateTimeOffset.UtcNow - count.StartTime;
			if (dif > _rule.TimeSpan)
			{
				await _cacheService.RemoveAsync(cacheKey);
				await _cacheService.AddAsync(cacheKey, new RequestsPerTimeSpanCount { StartTime = DateTimeOffset.UtcNow, Count = 1 });
				return new RuleRunResult
				{
					IsSuccess = true
				};
			}
			else
			{
				if (count.Count < _rule.AllowedNumberOfRequests)
				{
					count.Count++;
					await _cacheService.AddAsync(cacheKey, count);
					return new RuleRunResult
					{
						IsSuccess = true
					};
				}
				else
				{
					await Task.CompletedTask;
					return new RuleRunResult
					{
						IsSuccess = false,
						ErrorMessage = $"No of requests exceeded the allowed number {_rule.AllowedNumberOfRequests} in the last {_rule.TimeSpan.TotalMilliseconds}ms"
					};
				}
			}

		}
	}
}
