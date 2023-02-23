namespace RateLimiter.Constants
{
	public class ErrorMessages
	{
		public const string UnexpectedRule = "Unexpected rule";
		public const string NoRulesError = "It is no rules for {0} key";
		public const string TotalRequestCountLimitError = "You have reached out the limit of requests ({0})";
		public const string RequestsPerTimeLimiterError = "You have reached out the limit of requests in time range ({0} in {1})";
	}
}