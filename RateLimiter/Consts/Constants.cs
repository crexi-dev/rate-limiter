namespace RateLimiter.Consts
{
    public static class Constants
    {
        public const int MaxRequestsCount = 1000;

        public const string RequestsPerTimeSpanRuleMessage = "The maximum amount of requests has been exceeded";
        public const string TimeSpanSinceLastRequestRuleMessage = "Enough time has not elapsed since the last request";
        public const string TimeSpanSinceLastSuccessfulRequestRuleMessage = "Enough time has not elapsed since the last successful request";
    }
}
