using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RateLimiter.Services
{
    public class RateLimiterInMemoryRepository : IRateLimiterRepository
    {
        private Dictionary<string, SortedList<DateTime, int>> _cache = new Dictionary<string, SortedList<DateTime, int>>();

        private SortedList<DateTime, int> GetUserCache(string userToken)
        {
            SortedList<DateTime, int> userCache;

            if (!_cache.ContainsKey(userToken))
            {
                userCache = new SortedList<DateTime, int>(new DuplicateKeyComparer<DateTime>());
                _cache.Add(userToken, userCache);
            }
            else
            {
                userCache = _cache[userToken];
            }

            return userCache;
        }

        public void SaveUserRequest(string userToken, IDateTimeWrapper dateTime)
        {
            var userCache = GetUserCache(userToken);

            userCache.Add(dateTime.Now, 0);
        }

        public int CountRequests(string userToken, IDateTimeWrapper now, TimeSpan timeSpan)
        {
            var userCache = GetUserCache(userToken);
            var targetDate = now.Now - timeSpan;
            int requestCount = 0;

            foreach (var item in userCache)
            {
                if (item.Key > targetDate && item.Key <= now.Now)
                {
                    requestCount++;
                }
                else
                {
                    break;
                }
            }

            return requestCount;
        }

        //Simple cleanup to clear requests that no longer apply
        public void Cleanup(DateTime removeBeforeDate)
        {
            foreach (var kvp in _cache)
            {
                var sortedList = kvp.Value;
                var newSortedList = new SortedList<DateTime, int>(new DuplicateKeyComparer<DateTime>());

                foreach (var item in sortedList)
                {
                    if (item.Key > removeBeforeDate)
                    {
                        newSortedList.Add(item.Key, item.Value);
                    }
                }

                _cache[kvp.Key] = newSortedList;
            }
        }
    }

    public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1; // Handle equality as being greater. Note: this will break Remove(key) or
            else          // IndexOfKey(key) since the comparer never returns 0 to signal key equality
                return result;
        }

        #endregion
    }
}
