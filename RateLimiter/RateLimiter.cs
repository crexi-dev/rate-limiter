using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Data;
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
        private Dictionary<string, Resource> _resources;

        public Func<HttpRequestMessage, string> GetResourceId { get; set; } = (request) =>
        {
            return request.RequestUri.AbsolutePath;
        };

        public RateLimiter(Dictionary<string, Resource> resources) : base(new HttpClientHandler())
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
                var resource = _resources[id];
                var limiter = resource.GetLimiter(request);

                using RateLimitLease lease = await limiter.WaitAsync(request, 1, cancellationToken);
                if (lease.IsAcquired)
                {
                    isLease = true;
                }
                if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    retry_seconds = retryAfter;
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
