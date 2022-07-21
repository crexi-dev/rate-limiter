using RateLimiter.Model;

namespace RateLimiter.DataAccess
{
    public interface IStorageService
    {
        UserRequestCache GetToken(string key);
        void SetToken(string key, UserRequestCache token);
    }
}