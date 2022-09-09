namespace RateLimiter.Statistics
{
    public interface IClientStatistics
    {
        int NumberOfRequests { get; set; }
        DateTime LastSuccessfulResponseTime { get; set; }
    }
}
