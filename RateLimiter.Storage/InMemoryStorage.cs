using RateLimiter.Interfaces;
using RateLimiter.Models;
using System.Collections.Concurrent;

namespace RateLimiter.Repository
{
    public class InMemoryStorage : IRepository
    {

        private static readonly ConcurrentDictionary<string, RequestDetails> _inMemoryStore  = new ConcurrentDictionary<string, RequestDetails>();
        public ConcurrentDictionary<string, RequestDetails>  AccessStorage()
        {
            return _inMemoryStore;
        }

        public void  ClearStorage()
        {
            _inMemoryStore.Clear();
        }
    }
}
