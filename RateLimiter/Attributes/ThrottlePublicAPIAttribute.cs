using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RateLimiter.Rules;
using RateLimiter.Storage;

namespace RateLimiter.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ThrottlePublicAPIAttribute : ActionFilterAttribute
    {
        private static readonly Guid Resource = Guid.NewGuid(); 

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Headers["AccessToken"].FirstOrDefault();
            var requestsPerTimespanRule = new RequestsPerTimespan(10, 1);
            var statisticsRepository = new AccessStatisticsStaticRepository();
            var accessService = new AccessService(Resource, new IRule[] { requestsPerTimespanRule }, statisticsRepository);

            if (token != null && accessService.HasAccess(token))
            {
                context.Result = new ContentResult
                {
                    StatusCode = 429,
                    Content = "Too Many Requests"
                };
            }

            base.OnActionExecuting(context);
        }
    }
}