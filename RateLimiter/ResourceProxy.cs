using RateLimiter.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    // TODO: make proxy generic. Use Invoke()
    public class ResourceProxy : IResource
    {
        protected readonly IResource resource;
        public ResourceProxy(IResource resource)
        {
            this.resource = resource;
        }

        public virtual string GetResource(string request, string clientId)
        {
            return resource.GetResource(request, clientId);
        }
    }
}
