using Microsoft.AspNetCore.Mvc;
using RateLimiter.Filters;

namespace RateLimiter.Api.Attributes
{
    /// <summary>
    /// A max size attribute used create filters to decorate our controllers/actions
    /// </summary>
    public class MaxSizeThrottleAttribute : TypeFilterAttribute
    {
        public MaxSizeThrottleAttribute() : base(typeof(MaxSizeThrottleFilter))
        {

        }
    }
}
