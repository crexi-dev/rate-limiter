using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Cache
{
    public class CacheEntry
    {
        private readonly List<DateTime> _entries = new();

        public DateTime Last { get; set; }

        public DateTime GetFirstEntry() => !_entries.Any() ? default : _entries[0];

        public void Add(DateTime entry) => _entries.Add(entry);

        public int GetCount() => _entries.Count;

        public void RemoveFirst() => _entries.Remove(_entries[0]);
    }
}
