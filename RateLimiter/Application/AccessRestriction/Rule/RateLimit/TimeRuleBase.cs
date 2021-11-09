namespace RateLimiter.Application.AccessRestriction.Rule.RateLimit
{
    /// <summary>
    /// Base class for rules implementing time and region based restrictions
    /// </summary>
    public abstract class TimeRuleBase : Rule
    {
        /// <summary>
        /// Region code to be used in a derived class.
        /// </summary>
        public string RegionCode { get; set; }
    }
}