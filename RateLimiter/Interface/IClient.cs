
using System.Collections.Generic;

namespace RateLimiter.Interface
{
    public interface IClient
    {
        string Token { get; }
        string Region { get; set; }
        string Subscription {get;set;}

        List<IResourceRequest> resourceRequests { get; set; }
    }
}
