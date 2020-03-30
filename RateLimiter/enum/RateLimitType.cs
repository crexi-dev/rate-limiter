namespace RateLimiter
{
    public enum RateLimitType {
        Whitelist,
        RequestsPerTimespan,
        TimespanPassedSinceLastCall
    }    
}