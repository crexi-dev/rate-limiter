namespace RateLimiter.Rules.Settings
{
    public class RequestPerTimespanSettings
    {
        public int TimespanInMs { get; set; }
        public int Count { get; set; }
    }
}
