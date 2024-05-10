namespace RateLimiter.Models
{
    public class RuleConfigurationModel
    {
        public string Name { get; set; }
        public string Action { get; set; }
        public string Endpoint { get; set; }
        public int RequestPeriod { get; set; }
        public int RequestLimit { get; set; }
    }
}
