using RateLimiter.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.BL
{
    public class RequestRepository : IRequestRepository
    {
        private static ConcurrentDictionary<string, ICollection<Request>> _db = new ConcurrentDictionary<string, ICollection<Request>>();

        public Task Add(Request requestHistory)
        {
            _db.AddOrUpdate(requestHistory.GetIdentifier(), new List<Request> { requestHistory }, (id, history) =>
            {
                history.Add(requestHistory);
                return history;
            });
            return Task.CompletedTask;
        }

        public Task<ICollection<Request>> Get(string accessToken)
        {
            if(_db.TryGetValue(accessToken, out ICollection<Request> result))
            {
                return Task.FromResult(result);
            }

            return Task.FromResult<ICollection<Request>>(new List<Request>());
        }
    }
}
