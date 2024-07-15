using RateLimiter.Interfaces.DataAccess;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.DataAccess.Repositiories
{
    /// <summary>
    /// This implementation is local/in-memory. It is only suitable for demo purposes as its store is not distributed.
    /// <see cref="DistributedRateLimitRepository.cs">See notes in DistributedRateLimitRepository</see>
    /// </summary>
    public class InMemoryRateLimitRepository : IRateLimitRepository
    {
        private readonly ConcurrentDictionary<string, IDictionary<string, object>> _lookup =
            new ConcurrentDictionary<string, IDictionary<string, object>>();

        public async Task<IDictionary<string, object>?> Retrieve(string key)
        {
            var result = _lookup.ContainsKey(key)
                ? _lookup[key]
                : null;

            return result;
        }

        public async Task Update(string key, IDictionary<string, object> values)
        {
            _lookup[key] = values;
        }
    }
}
