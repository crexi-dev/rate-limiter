using System;
using RateLimiter.Interfaces;

namespace RateLimiter.Models; 

public class ClientRequest:IClientRequest {
    public string ResourceAccessed { get; set; }
    public DateTime DateOfAccess { get; set; }
}