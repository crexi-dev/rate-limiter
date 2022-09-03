using System;
using System.Collections.Generic;

namespace RateLimiter.Repositories
{
    // Would normally be MemoryCache
    // This would probably have concurrency issues
    public class ClientRequestRepository : IClientRequestRepository
    {
        public Dictionary<string, List<DateTime>> ClientRequests { get; set; } = new Dictionary<string, List<DateTime>>();

        public List<DateTime> Add(string clientId, DateTime requestTime) // clientToken would normally be a clientId obtained from the token
        {
            bool clientExists = ClientRequests.TryGetValue(clientId, out List<DateTime>? requestTimes);
            if (clientExists)
            {
                requestTimes.Add(requestTime);
                ClientRequests[clientId] = requestTimes;
            }

            if (!clientExists)
            {
                requestTimes = new List<DateTime>() { requestTime };
                ClientRequests.Add(clientId, requestTimes);
            }

            return ClientRequests[clientId];
        }

        // TODO: public void CleanUp() // this would be cleanup old requestTimes to not bloat this  
    }
}
