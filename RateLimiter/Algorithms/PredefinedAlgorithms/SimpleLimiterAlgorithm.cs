using System;
using System.Threading;
using Timer = System.Timers.Timer;

namespace RateLimiter.Algorithms.PredefinedAlgorithms;

public sealed class SimpleLimiterAlgorithm : IRateLimiterAlgorithm
{
    private long _counter;
    private readonly long _limit;
    private Timer _timer;

    public SimpleLimiterAlgorithm(long limit, TimeSpan refreshPeriod)
    {
        _counter = 0;
        _limit = limit;
        _timer = new Timer(refreshPeriod.TotalMilliseconds);
        _timer.Elapsed += (sender, e) =>
        {
            Interlocked.Exchange(ref _counter, 0);
        };
        _timer.Start();
    }

    public string AlgorithmName => "SimpleAlgorithm";

    public bool Accepted(string api, string token)
    {
        if (Interlocked.Read(ref _counter) < _limit)
        {
            Interlocked.Increment(ref _counter);
            return true;
        }
        return false;
    }
}
