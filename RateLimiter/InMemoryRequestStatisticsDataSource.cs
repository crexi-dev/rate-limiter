using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Contract
{
    internal class InMemoryRequestStatisticsDataSource : IRequestStatisticsDataSource
    {
        private readonly ConcurrentDictionary<Guid, List<Request>> clientRequestsCache = new ConcurrentDictionary<Guid, List<Request>>();    

        private InMemoryRequestStatisticsDataSource() { }

        public Task AddRequestAsync(Request request)
        {
            this.clientRequestsCache.AddOrUpdate(
                request.ClientId, 
                k => new List<Request>(), 
                (k, v) => { v.Add(request); return v; });

            return Task.CompletedTask;
        }

        public Task<int> GetNumberOfRequestsPassedAsync(Guid clientId, DateTime startTime, DateTime endTime)
        {
            // this may not be optimal from performance standpoint, need to think of how to improve that
            // we may use bucketing (like increment number of requests within a second/minute bucket, depending on needed accuracy)
            return this.clientRequestsCache.TryGetValue(clientId, out var clientRequests)
                ? Task.FromResult(clientRequests.Count(t => t.DateTime >= startTime && t.DateTime <= endTime))
                : Task.FromResult(0);
        }

        public Task<Request> GetMostRecentRequestAsync(Guid clientId)
        {
            if (this.clientRequestsCache.TryGetValue(clientId, out var clientRequests))
            {
                return Task.FromResult(clientRequests.OrderByDescending(t => t.DateTime).FirstOrDefault());
            }

            return Task.FromResult((Request)null);
        }

        // Singleton, but in real scenario this should be registered in container as singleton
        public static InMemoryRequestStatisticsDataSource Instance = new InMemoryRequestStatisticsDataSource();
    }
}
