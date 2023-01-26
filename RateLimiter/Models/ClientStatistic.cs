using System;
using System.Collections.Generic;

namespace RateLimiter.Models
{
    public class ClientStatistic
    {
        public List<DateTime> Requests { get; set; } = new List<DateTime>();
        public long RequestCount { get; set; }

        public Dictionary<string, List<DateTime>> PrefixRequest { get; set; } =
            new Dictionary<string, List<DateTime>>();
    }
}