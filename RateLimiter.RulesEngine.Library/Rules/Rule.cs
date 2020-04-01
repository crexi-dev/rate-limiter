using RateLimiter.Library;

namespace RateLimiter.RulesEngine.Library.Rules
{
    public abstract class Rule {
        public int Id { get; set; }
        public string Name { get; set; }
        public RateLimitType RateLimitType { get; set; }
        public RateLimitLevel RateLimitLevel { get; set; }
        public bool IsActive { get; set; }

        public Rule(string name, RateLimitType rateLimitType, RateLimitLevel rateLimitLevel)
        {
            this.Name = name;
            this.RateLimitType = rateLimitType;
            this.RateLimitLevel = rateLimitLevel;
            this.IsActive = false;
        }
    }

    public class ResourceRule : Rule
    {
        public string Resource { get; private set; }
        public Region Region { get; private set; }

        public ResourceRule(string name, string resource, Region region, RateLimitType rateLimitType, RateLimitLevel rateLimitLevel) : base(name, rateLimitType, rateLimitLevel)
        {
            this.Resource = resource;
        }

        public bool Match(string test)
        {
            return this.Resource == test;
        }
    }

    public class RegionRule : Rule
    {
        public Region Region { get; private set; }

        public RegionRule(string name, Region region, RateLimitType rateLimitType, RateLimitLevel rateLimitLevel) : base(name, rateLimitType, rateLimitLevel)
        {
            this.Region = region;
        }

        public bool Match(string test)
        {
            // match on ip address regex

            return false;
        }
    }
}