namespace RateLimiter.Models
{
    public enum RuleType : byte
	{
		RequestsPerTimeSpanRule,
		TimeSpanSinceLastCallRule
	}
}
