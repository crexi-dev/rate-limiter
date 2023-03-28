using System;
using System.Collections.Generic;

namespace RateLimiter;

public class Configuration
{
    public TimeSpan TimeSpan { get; set; }
    public int Limit { get; set; }
}