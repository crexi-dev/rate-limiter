using System;

namespace RateLimiter
{
    public class Token
    {
        public DateTime LastAccessTime { get; set; }
    }
    public enum Rules
    {
        LastAccessTime,
        AccessCounter
    }
}