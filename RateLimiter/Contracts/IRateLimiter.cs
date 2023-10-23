using System;
using System.Threading.Tasks;

namespace RateLimiter.Contracts
{
    public interface IRateLimiter
    {
        bool Validate(Guid token, DateTime utcTime, ResourcesType resource);

        Task<bool> ValidateAsync(Guid token, DateTime utcTime, ResourcesType resource);
    }
}
