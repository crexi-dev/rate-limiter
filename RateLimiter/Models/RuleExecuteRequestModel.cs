namespace RateLimiter.Models
{
    public class RuleExecuteRequestModel
    {
        public string Endpoint { get; set; }
        public string Name { get; set; }
        public long RequestPeriod { get; set; }
        public long RequestLimit { get; set; }
    }
}
