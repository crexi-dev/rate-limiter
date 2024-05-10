using RateLimiter.Classes;
using RateLimiter.Classes.Rules;

namespace RateLimitAPI
{
    public static class RateLimitSetup
    {
        public static RateLimitManager ConfigureRateLimiting()
        {
            var manager = new RateLimitManager();

            // Rule for weatherforecast: Up to 10 requests per minute
            manager.AddRule("/weatherforecast", new FixedNumberRule(10, TimeSpan.FromMinutes(1)));

            // Rule for Resource2: No more than 1 request per 30 seconds
            manager.AddRule("/api/resource2", new TimeSpanSinceLastCallRule(TimeSpan.FromSeconds(30)));

            // Rule for Resource3: Combination of FixedWindow and TimeSpan since last call
            manager.AddRule("/api/resource3", new FixedNumberRule(5, TimeSpan.FromMinutes(1)));
            manager.AddRule("/api/resource3", new TimeSpanSinceLastCallRule(TimeSpan.FromSeconds(10)));

            // Rule for Resource4: Geographic conditional rules
            var usRule = new FixedNumberRule(20, TimeSpan.FromMinutes(1));
            var euRule = new TimeSpanSinceLastCallRule(TimeSpan.FromMinutes(5));
            Func<string, string> getRegion = token => token.StartsWith("US") ? "US" : "EU";
            manager.AddRule("/api/resource4", new GeographicConditionalRule(usRule, euRule, getRegion));

            return manager;
        }
    }
}
