using System.Collections.Generic;
using System.Linq;
using RateLimiter.Constants;
using RateLimiter.Impl;
using RateLimiter.Interfaces;
using RateLimiter.Models;

namespace RateLimiter.Infrastructure
{
	public static class RequestValidator
	{
		public static ValidateModel ValidateRequest(RequestsData requestsData, List<ILimiterRule> limiterRules)
		{
			var errorLimiter =
				limiterRules
					.FirstOrDefault(o => !o.Validate(requestsData));

			return errorLimiter == null
				? new ValidateModel { IsValid = true }
				: new ValidateModel
					{
						IsValid = false,
						ErrorMessage = GetErrorMessage(errorLimiter)
					};
		}

		private static string GetErrorMessage(ILimiterRule rule)
		{
			switch (rule)
			{
				case TotalRequestCountLimit totalRequestCountLimit:
					return string.Format(ErrorMessages.TotalRequestCountLimitError, totalRequestCountLimit.CountLimit);
				
				case RequestsPerTimeLimiter requestsPerTimeLimiter:
					return string.Format(ErrorMessages.RequestsPerTimeLimiterError, requestsPerTimeLimiter.RequestLimit, requestsPerTimeLimiter.TimeRange);

				default:
					return ErrorMessages.UnexpectedRule;
			}
		}
	}
}