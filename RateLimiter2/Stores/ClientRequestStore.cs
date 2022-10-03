namespace RateLimiter.Stores
{
    public class ClientRequestStore
    {
        private class ResourceRequests
        {
            public Dictionary<string, List<DateTime>> ResourceRequestTimes = new Dictionary<string, List<DateTime>>();

            public bool Any(string resourceKey)
            {
                if (!ResourceRequestTimes.ContainsKey(resourceKey)) return false;
                return true;
            }

            public void Add(string resourceKey, DateTime timeStamp)
            {
                if (!ResourceRequestTimes.ContainsKey(resourceKey))
                    ResourceRequestTimes.Add(resourceKey, new List<DateTime>());

                ResourceRequestTimes[resourceKey].Add(timeStamp);
            }
        }

        private readonly Dictionary<string, ResourceRequests> ClientRequests = new Dictionary<string, ResourceRequests>();

        public bool Any(string clientKey, string resourceKey)
        {
            if (!ClientRequests.ContainsKey(clientKey)) return false;

            if(!ClientRequests[clientKey].Any(resourceKey)) return false;

            return true;
        }

        public void AddRequest(string clientKey, string resourceKey)
        {
            if (!ClientRequests.ContainsKey(clientKey))
                ClientRequests.Add(clientKey, new ResourceRequests());

            ClientRequests[clientKey].Add(resourceKey, DateTime.UtcNow);         
        }

        public List<DateTime> GetRequests(string clientKey, string resourceKey)
        {
            if (!Any(clientKey, resourceKey))
                return new List<DateTime>();

            return ClientRequests[clientKey].ResourceRequestTimes[resourceKey];
        }
    }
}
