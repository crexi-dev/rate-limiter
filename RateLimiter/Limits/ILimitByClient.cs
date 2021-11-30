namespace RateLimiter.Limits
{
    public interface ILimitByClient
    {
        bool CanInvoke(string clientId);
    }
}
