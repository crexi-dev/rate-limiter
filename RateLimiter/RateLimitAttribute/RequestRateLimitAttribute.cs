using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace RateLimitAttribute
{
    public class RequestRateLimitAttribute : Attribute
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;

            //var memoryCacheKey = $"{ip}-{Path}";
        }
    }
}