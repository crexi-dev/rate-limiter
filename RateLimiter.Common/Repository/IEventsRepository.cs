using RateLimiter.Models;
using RateLimiter.Repository;

namespace RateLimiter.Repository
{
    public interface IEventsRepository : IMemoryRepository<RequestCollection>
    {
    }
}
