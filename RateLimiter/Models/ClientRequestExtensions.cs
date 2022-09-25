namespace RateLimiter.Models
{
    public static class ClientRequestExtensions
	{
		public static string RequestsPerTimeSpanRuleCacheKey(this ClientRequest clientRequest)
		{
			return $"RequestsPerTimeSpanRuleCacheKey_{clientRequest.ClientKey}_{clientRequest.Resource}";
		}

		public static string TimeSpanSinceLastCallRuleCacheKey(this ClientRequest clientRequest)
		{
			return $"TimeSpanSinceLastCallRuleCacheKey_{clientRequest.ClientKey}_{clientRequest.Resource}";
		}
	}
}
