using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class RateLimiterAttribute : Attribute, IAsyncActionFilter
    {
        public IRateLimiterRequestService RequestService { get; set; }
        public IServiceProvider Provider { get; set; }

        public abstract Task<RateLimiterResponse> ExecuteLimiterAsync(RateLimiterRequest currentRequest);

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Provider = context.HttpContext.RequestServices;
            RequestService = Provider.GetRequiredService<IRateLimiterRequestService>();

            var request = GetRateLimiterRequest(context);
            request.Response = await ExecuteLimiterAsync(request);
            await RequestService.SaveRequestAsync(request);

            if (request.Response.WasAllowed)
            {
                await next();
            }
            else
            {
                string errorMsg = $"RateLimiter blocked request with statusCode: {request.Response.StatusCode}. Error message: {request.Response.ErrorMessage}";
                throw new InvalidOperationException(message: errorMsg);
            }
        }

        protected virtual RateLimiterRequest GetRateLimiterRequest(ActionExecutingContext context)
        {
            var controllerContext = (context.Controller as ControllerBase).ControllerContext;
            var controllerName = controllerContext.ActionDescriptor.ControllerName;
            var actionName = controllerContext.ActionDescriptor.ActionName;
            var request = new RateLimiterRequest { ActionName = actionName, ControllerName = controllerName };

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var countryClaim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == RateLimiterClaimTypes.COUNTRY);
                var regionClaim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == RateLimiterClaimTypes.REGION);
                request.Country = new RateLimiterRequestCountry { CountryName = countryClaim?.Value, RegionName = regionClaim?.Value };
            }

            request.RequestDate = DateTime.Now;
            return request;
        }
    }
}
