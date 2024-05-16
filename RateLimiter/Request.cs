
using System;

namespace RateLimiter;
public class Request
{
    public Token Token { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}