using System;

namespace RateLimiter
{
    [Flags]
    public enum HttpVerbFlags
    {
        Delete = 1,
        Get = 2,
        Patch = 4,
        Post = 8,
        Put = 16
    }
}
