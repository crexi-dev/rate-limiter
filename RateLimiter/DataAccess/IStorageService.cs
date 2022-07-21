using RateLimiter.Model;

namespace RateLimiter.Cache
{
    public interface IStorageService
    {
        UserRequestCache GetToken(string key);
        void SetToken(string key, UserRequestCache token);
    }
}