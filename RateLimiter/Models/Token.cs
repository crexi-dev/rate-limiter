using System;

namespace RateLimiter.Models;
public class Token
{
    public Guid ClientId
    {
        get; set;
    }
    public string Reqion { get; set; } = null!;
}