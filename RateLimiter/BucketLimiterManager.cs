using RateLimiter.Resources;
using RateLimiter.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class BucketLimiterManager
    {
        Dictionary<IResource, IResource> proxies = new Dictionary<IResource, IResource>();
        public void RegisterResource(IResource resource)
        {
            proxies.Add(resource, new RateLimiterProxy(resource));
        }
        public void UnRegisterResource(IResource resource) 
        {
            proxies.Remove(resource);
        }

        public void Reset()
        {
            proxies.Clear();
        }

        public void SetHandlers(IResource resource, List<ILimiterHandler> handlers)
        {
            if (!proxies.ContainsKey(resource)) return;

            var proxy = proxies[resource] as RateLimiterProxy;
            if(proxy is not null)
                proxy.SetHandlers(handlers);
        }

        public string GetResource(IResource resource, string request, string clientID)
        {
            if (proxies.ContainsKey(resource))
                return proxies[resource].GetResource(request, clientID);
            else
                return resource.GetResource(request, clientID);
        }
    }
}
