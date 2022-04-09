namespace RateLimiter.DataStorageSimulator
{
    /// <summary>
    /// Defines Rate Limiter types
    /// </summary>
    public enum RateLimiterType
    {
        rlUnknown = 0,
        rlRequestsPerInterval = 1, //Rate Limiter type, that limits the number of requests per time interval
        rlRequestsPerTimeout = 2   //Rate Limiter type, that declines request if it received too close to the previous request
    }
}
