using System;
using RateLimiter.Extensions;
using RateLimiter.Models;
using RateLimiter.Services.Interfaces;

namespace RateLimiter.Services;

/// <summary>
/// An implementation of IRateLimitingService
/// </summary>
/// <remarks>
/// Based on https://github.com/ThreeMammals/Ocelot/blob/develop/src/Ocelot/RateLimiting/RateLimiting.cs
/// Can be used in middleware, like in original ocelot repo: https://github.com/ThreeMammals/Ocelot/blob/develop/src/Ocelot/RateLimiting/Middleware/RateLimitingMiddleware.cs
/// Or inside an action filter
/// </remarks>
internal sealed class RateLimitingService : IRateLimitingService
{
    private static readonly TimeSpan DefaultRetryPeriod = TimeSpan.FromSeconds(1);
    private static readonly object ProcessLocker = new();

    private readonly IRateLimitStorageService _storageService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RateLimitingService(
        IRateLimitStorageService storageService,
        IDateTimeProvider dateTimeProvider)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public RateLimitCounter GetRateLimitCounter(ClientRequestIdentity identity, RateLimitRule rule)
    {
        RateLimitCounter counter;
        var counterId = identity.GetStorageKey(rule.Period);

        // Serial reads/writes from/to the storage which must be thread safe
        lock (ProcessLocker)
        {
            var entry = _storageService.Get(counterId);
            counter = Count(entry, rule);
            var expiration = rule.Period; // default expiration is set for the Period value

            if (counter.TotalRequests > rule.Limit)
            {
                var retryAfter = RetryAfter(counter, rule); // the calculation depends on the counter returned from CountRequests

                if (retryAfter > TimeSpan.Zero)
                {
                    // Rate Limit exceeded, ban period is active
                    expiration = rule.Period; // current state should expire in the storage after ban period
                }
                else
                {
                    // Ban period elapsed, start counting
                    _storageService.Remove(counterId); // the store can delete the element on its own using an expiration mechanism, but let's force the element to be deleted
                    counter = new RateLimitCounter(DateTime.UtcNow, null, 1);
                }
            }

            _storageService.Set(counterId, counter, expiration);
        }

        return counter;
    }

    private RateLimitCounter Count(RateLimitCounter? entry, RateLimitRule rule)
    {
        var now = _dateTimeProvider.UtcNow;
        if (!entry.HasValue) // no entry, start counting
        {
            return new RateLimitCounter(now, null, 1); // current request is the 1st one
        }

        var counter = entry.Value;
        var total = counter.TotalRequests + 1; // increment request count
        var startedAt = counter.StartedAt;
        if (startedAt + rule.Period >= now) // counting Period is active
        {
            var exceededAt = total >= rule.Limit && !counter.ExceededAt.HasValue // current request number equals to the limit
                ? now // the exceeding moment is now, the next request will fail but the current one doesn't
                : counter.ExceededAt;
            return new RateLimitCounter(startedAt, exceededAt, total); // deep copy
        }

        var wasExceededAt = counter.ExceededAt;
        return wasExceededAt + rule.Period >= now // ban PeriodTimespan is active
            ? new RateLimitCounter(startedAt, wasExceededAt, total) // still count
            : new RateLimitCounter(now, null, 1); // Ban PeriodTimespan elapsed, start counting NOW!
    }

    private TimeSpan RetryAfter(RateLimitCounter counter, RateLimitRule rule)
    {
        var periodTimespan = rule.Period < DefaultRetryPeriod
            ? DefaultRetryPeriod // allow values which are greater or equal to 1 second
            : rule.Period; // good value

        var now = _dateTimeProvider.UtcNow;

        if (counter.StartedAt + rule.Period >= now) // counting Period is active
        {
            return counter.TotalRequests < rule.Limit
                ? TimeSpan.Zero // happy path, no need to retry, current request is valid
                : counter.ExceededAt.HasValue
                    ? periodTimespan - (now - counter.ExceededAt.Value) // minus seconds past
                    : periodTimespan; // exceeding not yet detected -> let's ban for whole period
        }

        if (counter.ExceededAt.HasValue // limit exceeding was happen
            && counter.ExceededAt + periodTimespan >= now) // ban PeriodTimespan is active
        {
            var startedAt = counter.ExceededAt.Value; // ban period was started at
            var secondsPast = now - startedAt;
            var retryAfter = periodTimespan - secondsPast;
            return retryAfter; // it can be negative, which means the wait in PeriodTimespan seconds has ended
        }

        return TimeSpan.Zero;
    }
}
