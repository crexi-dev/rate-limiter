using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RateLimiter.Models;

namespace RateLimiter.Repositories.Interfaces;

public interface IRateLimitRepository
{
    Task AddAsync(RateLimitRequestModel model, TimeSpan? expirationTime = null);
    Task<Dictionary<DateTime, RateLimitRequestModel>> GetAsync(string cacheKey);
}