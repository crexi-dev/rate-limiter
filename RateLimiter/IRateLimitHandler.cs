using Core.Models;

namespace RateLimiter
{
    public interface IRateLimitHandler
    {
        public bool IsRateLimitSucceded(RateLimitDecorator decorator, string key);
        public void UpdateClientData(RateLimitDecorator decorator, string key);
        public ClientData GetClientDataByKey(string key);
    }
}