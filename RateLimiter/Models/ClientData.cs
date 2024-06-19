using System;

namespace RateLimiter.Models;

public struct ClientData
{
    public DateTime LastVisit { get; set; }
    public int VisitCounts { get; set; }

    public static ClientData Empty => new()
    {
        LastVisit = DateTime.UtcNow,
        VisitCounts = 0
    };
}