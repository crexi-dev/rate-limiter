
using System;

namespace RateLimiter.Models;
public class Request
{
    public string Resource { get; init; } = null!;
    public Token Token { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}