using RateLimiter.Model;
using System;
using System.Collections.Generic;

namespace RateLimiter.Repositories
{
    public interface IRuleRepository
    {
        void Add(TimeSpan period, int limit);

        List<RateLimiterRule> GetAll();
    }
}
