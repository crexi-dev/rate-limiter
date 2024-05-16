using System;

namespace RateLimiter;
public class Token
{
    public Guid ClientId{
        get; set;
    }
    public string Reqion{get;set;} = null!;
}