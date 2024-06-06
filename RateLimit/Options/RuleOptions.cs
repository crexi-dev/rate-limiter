namespace RateLimit.Options
{
	public class RuleOptions
	{
		public const string RuleOption = "RuleOption";

		public int Limit { get; set; }
		public TimeSpan Period { get; set; }
		public TimeSpan LastCallPeriod { get; set; }
	}
}
