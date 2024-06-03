var rateLimiter = new RateLimiter();

var fixedRule = new FixedWindowRateLimitRule(4, TimeSpan.FromMinutes(1));
var slidingRule = new SlidingWindowRateLimitRule(TimeSpan.FromSeconds(1));
var regionRules = new Dictionary<string, IRateLimitRule>
{
    { "US", new FixedWindowRateLimitRule(5, TimeSpan.FromSeconds(1)) },
    { "EU", new SlidingWindowRateLimitRule(TimeSpan.FromSeconds(1)) }
};
var regionRule = new RegionBasedRateLimitRule(regionRules);
 
rateLimiter.AddRule("resource1", fixedRule);
rateLimiter.AddRule("resource2", slidingRule);
rateLimiter.AddRule("resource3", fixedRule);
rateLimiter.AddRule("resource3", slidingRule);
rateLimiter.AddRule("resource4", regionRule);

var client = "USClient123";
var resource = "resource4";

for (int i = 0; i < 12; i++)
{
    Thread.Sleep(100);
    if (rateLimiter.HandleRequest(client, resource))
    {
        Console.WriteLine("Request allowed.");
    }
    else
    {
        Console.WriteLine("Request denied due to rate limit.");
    }
}
