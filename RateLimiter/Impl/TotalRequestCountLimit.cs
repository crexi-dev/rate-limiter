using RateLimiter.Interfaces;
using RateLimiter.Models;

namespace RateLimiter.Impl
{
	public class TotalRequestCountLimit: ILimiterRule
	{
		public int CountLimit { get; set; }

		public bool Validate(RequestsData requestsData)
		{
			return requestsData.Requests.Count <= CountLimit;
		}
	}
}