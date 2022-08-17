namespace RateLimiter.Models.Conditions
{
    public enum ConditionType
    {
        None = 0,
        RegionCondition = 1,
        IsClientAuthenticatedCondition = 2,
        HasRoleCondition = 3,
        HasSubscriptionCondition = 4
    }
}
