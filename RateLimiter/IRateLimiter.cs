using System;
using System.Threading.Tasks;

namespace RateLimiter
{
    public interface IRateLimiter : IDisposable
    {
        bool Allowed { get; }
        Task<bool> TryCallAsync();
    }
}
