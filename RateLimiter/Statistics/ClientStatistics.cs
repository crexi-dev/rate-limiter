namespace RateLimiter.Statistics
{
    internal class ClientStatistics : IClientStatistics
    {
        public int NumberOfRequests { get; set; }
        public DateTime LastSuccessfulResponseTime { get; set; }
    }
}
