
namespace RateLimiter
{
    public interface IRateLimitRule
    {
        bool IsLimitExceeded(string endpoint, string clientIdentifier);
        void RecordRequest(string endpoint, string clientIdentifier);
    }
}
