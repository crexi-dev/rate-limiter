using Microsoft.AspNetCore.Mvc;
using RateLimiter.Filters;

namespace RateLimiter.Api.Attributes
{
    /// <summary>
    /// A max requests attribute used create filters to decorate our controllers/actions
    /// </summary>
    public class MaxRequestsThrottleAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <returns></returns>
        public MaxRequestsThrottleAttribute() : base(typeof(MaxRequestsThrottleFilter))
        {

        }
    }
}
