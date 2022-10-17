
namespace RateLimiter.DataModel
{
    public class ClientResourceRuleMapping
    {
        public int ClientId { get; set; }
        public int ResourceId { get; set; }
        public int RuleId { get; set; }
        public int Value { get; set; }
    }
}
