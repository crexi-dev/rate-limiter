namespace RateLimiter.Library
{
    public enum RateLimitType {
        Whitelist,
        RequestsPerTimespan,
        TimespanPassedSinceLastCall
    }    
}