using System;
using System.Collections.Generic;

namespace RateLimiter.Classes
{
    // A memory storage to simulate a database
    public static class MemoryStore
    {
        public static Dictionary<string, List<DateTime>> Requests = new();
    }
}
