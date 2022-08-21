using System;

namespace RateLimiter.DataStore{

public class RuleBStore
{
    public string Token { get; set; } = null!;
    public DateTime LastRequestTimeSpan { get; set; }
}
}