

namespace RateLimiter.Interfaces
{
    public interface IRequestLimitter
    {
        void Configure(IAllowRequest rule);
        bool Validate(string strValidate);
    }
}
