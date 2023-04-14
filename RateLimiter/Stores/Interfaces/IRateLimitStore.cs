using System.Collections.Generic;
using System.Threading.Tasks;
using RateLimiter.Stores.Models;

namespace RateLimiter.Stores.Interfaces;

public interface IRateLimitStore
{
    Task Add(string resource);
    Task<List<RequestRateModel>> Get(string resource);
    
    Task<RequestRateModel?> GetLast(string resource);
}