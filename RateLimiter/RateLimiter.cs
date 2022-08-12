using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiter : DelegatingHandler
    {        
        private Dictionary<string, List<ReplenishingRateLimiter>> _resources;
        
        public Func<HttpRequestMessage, string> GetResourceId { get; set; } = (request) =>
        {
            return request.RequestUri.AbsolutePath;
        };
      
        public RateLimiter(Dictionary<string, List<ReplenishingRateLimiter>> resources) : base(new HttpClientHandler())
        {
            _resources = resources;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {           
            var id = GetResourceId(request);
            var isLease = false;
            TimeSpan? retry_seconds = null;

            if (_resources.ContainsKey(id))
            {
                var rules = _resources[id];
                if (rules != null && rules.Count > 0)
                {
                    //TODO: This could impact performance if number of rules are big
                    foreach (var rule in rules)
                    {
                        using RateLimitLease lease = await rule.WaitAsync(1, cancellationToken);
                        if (lease.IsAcquired)
                        {
                            isLease = true;
                            break;
                        }
                        if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                        {
                            retry_seconds = retryAfter;
                        }
                    }
                }
            }
            if (isLease)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.TooManyRequests);
            if (retry_seconds != null)
            {
                response.Headers.Add(HeaderNames.RetryAfter, ((int)retry_seconds.Value.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
            }
            return response;
        }


    }
}
