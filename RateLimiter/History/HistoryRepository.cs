using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RateLimits.History
{
    public class HistoryRepository : IHistoryRepository
    {
        private ConcurrentDictionary<string, List<HistoryModel>> _history = new ConcurrentDictionary<string, List<HistoryModel>>();

        public List<HistoryModel> Get(string accessToken, string resource)
        {
            var answer = new List<HistoryModel>();
            if (_history.ContainsKey($"{accessToken}_{resource}"))
                answer = _history[$"{accessToken}_{resource}"];
            return answer;
        }

        public void Add(string accessToken, string resource, string region)
        {
            var newRecord = new HistoryModel(DateTime.Now, region);
            _history.AddOrUpdate($"{accessToken}_{resource}", new List<HistoryModel> { newRecord }, (x, y) =>
             {
                 //Remove old history which is no longer needed
                 var toRemove = y.Where(z => z.Date < DateTime.Now.AddMonths(-1));
                 foreach (var item in toRemove)
                 {
                     y.Remove(item);
                 }
                 //Add new record in history
                 y.Add(newRecord);
                 return y;
             });
        }
    }
}
