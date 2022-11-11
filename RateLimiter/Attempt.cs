namespace RateLimiter;

public sealed class Attempt
{
    public long Date { get; }
    public IReadOnlyCollection<RateLimiterParameter>? Parameters { get; }

    public Attempt(DateTime date, IReadOnlyCollection<RateLimiterParameter>? parameters = null)
    {
        Date = date.Ticks;
        Parameters = parameters;
    }
    public Attempt(long date, IReadOnlyCollection<RateLimiterParameter>? parameters = null)
    {
        Date = date;
        Parameters = parameters;
    }
}