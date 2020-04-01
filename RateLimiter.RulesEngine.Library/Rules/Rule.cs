using RateLimiter.Library;

namespace RateLimiter.RulesEngine.Library.Rules
{
    public abstract class Rule {
        public int Id { get; private set; }
        public string Name { get; private set; }
        //public RuleType RuleType { get; private set; }
        public RateLimitType RateLimitType { get; private set; }
        public RateLimitLevel RateLimitLevel { get; private set; }

        public Rule(string name, RateLimitType rateLimitType, RateLimitLevel rateLimitLevel)
        {
            this.Name = name;
            //this.RuleType = RuleType;
            this.RateLimitType = rateLimitType;
            this.RateLimitLevel = rateLimitLevel;
        }
    }

    public class ResourceRule : Rule
    {
        public string ResourceName { get; private set; }

        public ResourceRule(string name, string resourceName, RateLimitType rateLimitType, RateLimitLevel rateLimitLevel) : base(name, rateLimitType, rateLimitLevel)
        {

        }

        public bool Match(string test)
        {
            return this.ResourceName == test;
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

    public class ResourceRegionRule : Rule
    {
        public string Resource { get; private set; }
        public Region Region { get; private set; }

        public ResourceRegionRule(string name, string resouce, Region region, RateLimitType rateLimitType, RateLimitLevel rateLimitLevel) : base(name, rateLimitType, rateLimitLevel)
        {
            this.Resource = resouce;
            this.Region = region;
        }

        public bool Match(string resourceName, string IPAddress)
        {
            // match on resource name and ip address regex

            return false;
        }
    }
}