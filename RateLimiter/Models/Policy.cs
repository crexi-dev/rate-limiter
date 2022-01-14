using System;

namespace RateLimiter.Models;

public class Policy
{
    public string? Path { get; set; }
    public TimeSpan Timeout { get; set; }
    public int Count { get; set; }
}