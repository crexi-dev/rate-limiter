using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RateLimiter.Models;

namespace RateLimiter.Abstractions;

public interface IUserActivityRepository
{
    Task<IQueryable<UserActivity>> GetAllQueryableAsync(
        Expression<Func<UserActivity, bool>>? predicate = null,
        Expression<Func<UserActivity, UserActivity>> selector = null);
}