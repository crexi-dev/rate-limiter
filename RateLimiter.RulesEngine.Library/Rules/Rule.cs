namespace RateLimiter.RulesEngine.Library.Rules
{
    public class Rule {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public RateLimitType RateLimitType { get; private set; }
        public RateLimitLevel RateLimitLevel { get; private set; }

        public Rule(string name, RateLimitType rateLimitType, RateLimitLevel rateLimitLevel)
        {
            this.Name = name;
            this.RateLimitType = rateLimitType;
            this.RateLimitLevel = rateLimitLevel;
        }
    }
}