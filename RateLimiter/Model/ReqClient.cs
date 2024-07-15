using RateLimiter.Interface;
using System.Collections.Generic;

namespace RateLimiter.Model
{
    public class ReqClient : IClient
    {
        public ReqClient(string token) { Token = token; }
        public string Token { get; }

        public string Region { get; set ; }
        public string Subscription { get; set ; }
        public List<IResourceRequest> resourceRequests { get ; set ; } = new List<IResourceRequest> ();
    }
}
