namespace RateLimiter
{
    public interface IRateLimitLibrary
    {
        bool IsRateLimitAccepted(string userToken, string apiName);
    }
}
