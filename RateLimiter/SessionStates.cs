namespace RateLimiter
{
    public static class SessionState
    {
        /// <summary>
        /// Client limited to X API calls per X seconds  
        /// </summary>
        public const int RateLimit = 5;

        /// <summary>
        /// Client limit reset after X seconds
        /// </summary>
        public const double RateTimeLimit = 5000;
    }
}
