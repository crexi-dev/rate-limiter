namespace RateLimiter.Models
{
    public class RateLimitRuleModel
    {
        public string Endpoint { get; set; }
        public int RequestPeriod { get; set; }
        public int RequestLimit { get; set; }
    }
}
