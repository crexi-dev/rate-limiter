namespace RateLimiter.Model
{
    public class RuleResult
    {
        public bool Allow { get; set; }
        public string RuleName { get; set; }
        public string Message { get; set; }
    }
}
