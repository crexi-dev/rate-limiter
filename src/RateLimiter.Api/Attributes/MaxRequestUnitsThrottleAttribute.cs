using Microsoft.AspNetCore.Mvc;
using RateLimiter.Filters;

namespace RateLimiter.Api.Attributes
{
    /// <summary>
    /// A max request units attribute used create filters to decorate our controllers/actions
    /// </summary>
    public class MaxRequestUnitsThrottleAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <returns></returns>
        public MaxRequestUnitsThrottleAttribute() : base(typeof(MaxRequestUnitsThrottleFilter))
        {

        }
    }
}
