namespace RateLimiter.Core.Tests
{
    public class RequestTimeline
    {
        public RequestTimeline(int delayMS, bool expected, string authToken)
        {
            DelayMS = delayMS;
            Expected = expected;
            AuthToken = authToken;
        }

        public string AuthToken { get; }
        public int DelayMS { get; }
        public bool Expected { get; }
    }
}