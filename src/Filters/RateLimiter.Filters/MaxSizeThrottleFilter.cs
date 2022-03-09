using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using RateLimiter.Models.Contract;
using RateLimiter.Models.Domain;

namespace RateLimiter.Filters
{
    /// <summary>
    /// Action filter used to validate the size of data accrued
    /// </summary>
    public class MaxSizeThrottleFilter : IAsyncActionFilter
    {
        private readonly IConfiguration _configuration;

        public MaxSizeThrottleFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Called before/after the action executes.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var clientMetrics = (ClientMetrics)context.HttpContext.Items["ClientMetrics"];

            var maxSize = _configuration.GetValue<int>("MaxSize");
            if (clientMetrics.TotalSize >= maxSize)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Result = new JsonResult(new ErrorResponse("Rate limit is exceeded.",
                    "You have reached a limit of the cost of operations in the given period."));
                return;
            }

            await next();
        }
    }
}
