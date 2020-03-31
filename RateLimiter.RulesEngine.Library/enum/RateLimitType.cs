namespace RateLimiter.RulesEngine.Library
{
    public enum RateLimitType {
        Whitelist,
        RequestsPerTimespan,
        TimespanPassedSinceLastCall
    }    
}