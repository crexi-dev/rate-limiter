using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.InMemoryStore
{
    public interface ICacheStore
    {
        Task SetAsync(ClientRequest clientRequest);

        Task<List<DateTime>> GetAsync(ClientRequest clientRequest);
    }
}
