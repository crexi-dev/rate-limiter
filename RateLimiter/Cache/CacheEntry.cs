namespace RuleLimiterTask.Cache
{
    public class CacheEntry
    {
        private List<DateTime> _entries = new();

        public DateTime Last { get; set; }

        public DateTime GetLastEntry() => !_entries.Any() ? default : _entries[^1];

        public void Add(DateTime entry) => _entries.Add(entry);

        public int GetCount() => _entries.Count;

        public void Clear() => _entries.Clear();

        public int GetCountByTimespan(DateTime requestTime, TimeSpan timeSpan) => _entries.Count(x => requestTime - x < timeSpan);

        public void RemoveFirst() => _entries.Remove(_entries[0]);
    }
}
