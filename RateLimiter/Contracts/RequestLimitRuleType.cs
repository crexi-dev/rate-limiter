namespace RateLimiter.Contracts
{
    public enum RequestLimitRuleType : byte
    {
        RequestsPerTime = 1,
        TiemAfterLastCall = 2
    }
}
