namespace RateLimiter;

public sealed class RateLimiterRule
{
    public int Limit { get; }
    public long Interval { get; }
    public IReadOnlyCollection<RateLimiterParameter>? Parameters { get; }

    public RateLimiterRule(TimeSpan interval, int limit, IEnumerable<RateLimiterParameter>? parameters = null)
    {
        if (interval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), "Interval can not be less or equal Zero");
        }

        if (limit < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit can not me less then 1");
        }

        Interval = interval.Ticks;
        Limit = limit;
        Parameters = parameters?.ToList();
    }
}