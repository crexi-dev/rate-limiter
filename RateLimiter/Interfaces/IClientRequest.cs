using System;

namespace RateLimiter.Interfaces;

public interface IClientRequest {
    public string ResourceAccessed { get; set; }
    public DateTime DateOfAccess { get; set; }
}