using System;
using System.Collections.Generic;

namespace RateLimiter.Stores.Repositories
{
    // Would normally be MemoryCache
    // This would probably have concurrency issues
    public class ClientRequestRepository : IClientRequestRepository
    {
        public Dictionary<string, List<DateTime>> ClientRequests { get; set; } = new Dictionary<string, List<DateTime>>();

        public T Get<T>(string clientId) where T : class
        {
            bool clientExists = ClientRequests.TryGetValue(clientId, out List<DateTime> requestTimes);
            if (!clientExists)
            {
                throw new KeyNotFoundException(string.Format("Key: '{0}' not found", clientId));
            }

            return requestTimes as T;
        }

        public void Set<T>(string clientId, T newRequestTimes) where T : class
        {
            bool clientExists = ClientRequests.TryGetValue(clientId, out _);
            if (!clientExists)
            {
                ClientRequests.Add(clientId, newRequestTimes as List<DateTime>);
            }

            if (clientExists)
            {
                ClientRequests[clientId] = newRequestTimes as List<DateTime>;
            }
        }

        // TODO: public void CleanUp() // this would be cleanup old requestTimes to not bloat this  
    }
}
