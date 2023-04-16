using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using RateLimiter.Models;

namespace RateLimiter.Rules;

public class RequestsPerTimespanRule : IRule<RequestData>, IDisposable
{
    protected readonly TimeSpan RequestTimeSpan;
    protected readonly int AllowedRequestCount;
    private readonly ConcurrentQueue<DateTime> _requestHistory;
    private readonly SemaphoreSlim _semaphore;
    private readonly Timer _timer;
    
    public RequestsPerTimespanRule(TimeSpan requestTimeSpan, int requestCount)
    {
        RequestTimeSpan = requestTimeSpan;
        AllowedRequestCount = requestCount;
        _requestHistory = new ConcurrentQueue<DateTime>();
        _semaphore = new SemaphoreSlim(0, AllowedRequestCount);
        _timer = new Timer(TimerCallbackFunc, _semaphore, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
    }

    public virtual async Task<bool> CheckAsync(RequestData obj, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        _requestHistory.Enqueue(DateTime.UtcNow);
        return true;
    }

    public string Description => $"Rule {AllowedRequestCount} requests per {RequestTimeSpan}";

    private void TimerCallbackFunc(object? objInfo)
    {
        if (_requestHistory.IsEmpty && _semaphore.CurrentCount != AllowedRequestCount)
        {
            _semaphore.Release(AllowedRequestCount);
            return;
        }

        while (_requestHistory.TryPeek(out DateTime oldestDate) 
               && oldestDate <= DateTime.UtcNow.Subtract(RequestTimeSpan)
               && _requestHistory.TryDequeue(out _)
               && _semaphore.CurrentCount != AllowedRequestCount)
        {
            _semaphore.Release();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _semaphore.Dispose();
            _timer.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}