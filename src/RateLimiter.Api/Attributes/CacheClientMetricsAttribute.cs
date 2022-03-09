using Microsoft.AspNetCore.Mvc;
using RateLimiter.Filters;

namespace RateLimiter.Api.Attributes
{
    /// <summary>
    /// A caching attribute used create filters to decorate our controllers/actions
    /// </summary>
    public class CacheClientMetricsAttribute : TypeFilterAttribute
    {
        public CacheClientMetricsAttribute() : base(typeof(CacheClientMetricsFilter))
        {

        }
    }
}
