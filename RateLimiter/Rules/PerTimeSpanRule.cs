using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public class PerTimeSpanRule : IRule
    {
        public int Count { get; }
        public TimeSpan TimeSpan { get; }

        private readonly MemoryCache _cache;

        public PerTimeSpanRule(int count, TimeSpan timeSpan)
        {
            Count = count;
            TimeSpan = timeSpan;

            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool IsAllowed(string token)
        {
            var now = DateTime.UtcNow;
            var item = _cache.GetOrCreate(token, x => new Queue<DateTime>());

            if (QueueIsOk(item, now))
            {
                item.Enqueue(now);
                return true;
            }

            return false;
        }

        private bool QueueIsOk(Queue<DateTime> queue, DateTime date)
        {
            while (queue.TryPeek(out var ealriestDate))
            {
                if (date - ealriestDate < TimeSpan)
                {
                    break;
                }

                queue.Dequeue();
            }

            return queue.Count < Count;
        }
    }
}
