namespace RateLimiter.Domain.ApiLimiter
{
    public interface IApiLimiter
    {
        bool Verify(string resource, string token);
    }
}