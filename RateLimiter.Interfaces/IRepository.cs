using RateLimiter.Models;
using System.Collections.Concurrent;

namespace RateLimiter.Interfaces
{
    public interface IRepository
    {
        public ConcurrentDictionary<string, RequestDetails> AccessStorage();
        public void ClearStorage();
    }
}
