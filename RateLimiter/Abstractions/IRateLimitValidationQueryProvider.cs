using System.Linq;
using RateLimiter.Models;

namespace RateLimiter.Abstractions;

public interface IRateLimitValidationQueryProvider
{
    IQueryable<UserActivity> ApplyFilter(IRateLimit rateLimit, IQueryable<UserActivity> queryable);
}