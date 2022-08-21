using System;

namespace RateLimiter.DataStore{

public class RuleAStore
{
    public string Token { get; set; } = null!;
    public DateTime RequestTimeSpan { get; set; }
    public int RemainingRequestsInTimeSpan { get; set; }
}
}