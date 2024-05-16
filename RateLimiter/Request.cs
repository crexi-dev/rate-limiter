
using System;

namespace RateLimiter;
public class Request
{
    public string Resource { get; init;} = null!;
    public Token Token { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}