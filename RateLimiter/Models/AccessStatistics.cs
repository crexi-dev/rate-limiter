using System;
using System.Collections.Generic;

namespace RateLimiter.Models;

public class AccessStatistics(string token, Guid resource)
{
    public List<Access> AccessList { get; set; } = new();
}

public class Access
{
    public DateTimeOffset AccessTime { get; set; }
}