using System;

namespace RateLimiter.Interfaces;
public interface IRule
{
    public bool IsRequestAllowed(string token);
}
