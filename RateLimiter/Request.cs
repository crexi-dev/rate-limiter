using System;

namespace RateLimiter;

public class Request
{
    public Guid ClientId { get; set; }
    public DateTime CreateTime { get; set; }
}