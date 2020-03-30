using System;

namespace RateLimiter
{
    internal interface IRateLimiter {
        bool Verify(string token, DateTime requestDate, string serverIP);
    } 
}