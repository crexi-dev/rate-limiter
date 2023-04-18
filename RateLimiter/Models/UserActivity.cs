using System;

namespace RateLimiter.Models;

public class UserActivity
{
    public string ApiKey { get; set; }

    public DateTime Timestamp { get; set; }
}