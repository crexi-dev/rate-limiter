using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RateLimiter.Abstractions;
using RateLimiter.Models;

namespace RateLimiter.Fakes;

public class FakeUserActivityRepository : IUserActivityRepository
{
    public Task<IQueryable<UserActivity>> GetAllQueryableAsync(
        Expression<Func<UserActivity, bool>>? predicate = null,
        Expression<Func<UserActivity, UserActivity>>? selector = null)
    {
        var result = FakeUserActivityData.AsQueryable();
        if (predicate != null) result = result.Where(predicate);

        if (selector != null) result = result.Select(selector);

        return Task.FromResult(result);
    }

    private static IEnumerable<UserActivity> FakeUserActivityData => new List<UserActivity>
    {
        new() { ApiKey = "too-many-requests-in-month", Timestamp = DateTime.Now.AddDays(-5) },
        new() { ApiKey = "too-many-requests-in-month", Timestamp = DateTime.Now.AddDays(-10) },
        new() { ApiKey = "too-many-requests-in-month", Timestamp = DateTime.Now.AddDays(-15) },
        new() { ApiKey = "too-many-requests-in-month", Timestamp = DateTime.Now.AddDays(-30) },

        new() { ApiKey = "not-enough-wait", Timestamp = DateTime.Now },

        new() { ApiKey = "no-restrictions", Timestamp = DateTime.Now.AddHours(1) }
    };
}