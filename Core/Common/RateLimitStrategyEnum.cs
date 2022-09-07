namespace Core.Common
{
    public enum RateLimitStrategyEnum : byte
    {
        LimitedRequestPerTimespan,
        CertainTimespanPassedSinceLastCall,
    }
}
