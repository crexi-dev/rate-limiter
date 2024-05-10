namespace RateLimiter.Models
{
    public class ReadRuleResponseModel
    {
        public string Endpoint { get; set; }
        public string Name { get; set; }
        public int RequestPeriod { get; set; }
        public int RequestLimit { get; set; }
    }
}
