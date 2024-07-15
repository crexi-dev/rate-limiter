using RateLimiter.Interfaces.DataAccess;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.DataAccess.Repositiories
{
    // TODO: Implement this repo in order to support horizontal scaling for load balancing scenarios. In this
    // scenario, all server nodes need synchronized access to the same repo. Suggest Redis or similar.
    public class DistributedRateLimitRepository : IRateLimitRepository
    {
        public async Task<IDictionary<string, object>?> Retrieve(string key)
        {
            throw new NotImplementedException();
        }

        public async Task Update(string key, IDictionary<string, object> values)
        {
            throw new NotImplementedException();
        }
    }
}
