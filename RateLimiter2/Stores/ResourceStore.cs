using RateLimiter.Resources;
using RateLimiter.Rules;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Stores
{
    internal class ResourceStore
    {
        public readonly IEnumerable<Resource> Resources;

        public ResourceStore()
        {
            // TODO - Load rules and resources from settings or persistent storge

            const string Five_per_second = "5 per second";
            const string Ten_per_second = "10 per second";
            const string Thousand_per_min = "1000 per minute";
            const string One_per_day = "1 per day";
            const string Idle_1_second = "idle 1 second";
            const string Idle_3_seconds = "idle 3 seconds";
            const string Idle_10_seconds = "idle 10 seconds";
            const string Idle_1_minute = "idle 1 min";
            const string Idle_300_ms = "idle 300 ms";
            const string US_Region = "US";
            const string EU_Region = "EU";

            var rules = new List<BaseRule>()
            {
                new MaximumRequestRateRule { Name = Five_per_second, RequestCount = 5, ElapsedTime = new TimeSpan(0, 0, 1) },
                new MaximumRequestRateRule { Name = Ten_per_second, RequestCount = 10, ElapsedTime = new TimeSpan(0, 0, 1) },
                new MaximumRequestRateRule { Name = Thousand_per_min, RequestCount = 1000, ElapsedTime = new TimeSpan(0, 1, 0) },
                new MaximumRequestRateRule { Name = One_per_day, RequestCount = 1, ElapsedTime = new TimeSpan(23, 0, 0) },
                new MinimumIdleTimeRule { Name = Idle_1_second, IdleTime = new TimeSpan(0, 0, 1) },
                new MinimumIdleTimeRule { Name = Idle_3_seconds, IdleTime = new TimeSpan(0, 0, 3) },
                new MinimumIdleTimeRule { Name = Idle_10_seconds, IdleTime = new TimeSpan(0, 0, 10) },
                new MinimumIdleTimeRule { Name = Idle_300_ms, IdleTime = new TimeSpan(0, 0, 0, 0, 300) },

                new TokenInRegionRule { Name = US_Region, Region = "US", SortOrder = 0,
                    RegionalRule = new MaximumRequestRateRule { Name = Five_per_second, RequestCount = 5, ElapsedTime = new TimeSpan(0, 0, 5) } },

                new TokenInRegionRule { Name = EU_Region, Region = "EU", SortOrder = 0,
                    RegionalRule = new MinimumIdleTimeRule { Name = Idle_300_ms, IdleTime = new TimeSpan(0, 0, 0, 0, 300)} }
            };

            Resources = new List<Resource>()
            {
                new Resource { Path = "/api/customer", HttpVerb = "GET", Rules = rules.Where(r => r.Name == Five_per_second || r.Name == Idle_300_ms) },
                new Resource { Path = "/api/login", HttpVerb = "GET", Rules = rules.Where(r => r.Name == Idle_1_minute) },
                new Resource { Path = "/api/rates", HttpVerb = "GET", Rules = rules.Where(r => r.Name == US_Region) },
                new Resource { Path = "/api/rates", HttpVerb = "POST", Rules = rules.Where(r => r.Name == Idle_1_minute) },
            };
        }
    }
}
