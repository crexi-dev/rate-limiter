using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLitimer.WebApi.Tools
{
    public class CustomActionFilters : ActionFilterAttribute
    {
        private static MemoryCache MemoryCache { get; } = new MemoryCache(new MemoryCacheOptions()); // Make it static so the value will stay

        public CustomActionFilters()
        {

        }
       
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpRequest httpRequest = context.HttpContext.Request;
            StringValues authValues = httpRequest.Headers["Authorization"];

            if (authValues.Count > 0)
            {
                string key = ("" + authValues.ToString()).Trim();

                // To todo
                // validate the key of the client in the database if it is really exisiting

                if(key != string.Empty)
                {
                    MemoryCache.TryGetValue(key, out bool keyDoesExists);

                    if (!keyDoesExists)
                    {
                        MemoryCache.Set(key, true, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMilliseconds(5000) });
                    }
                    else
                    {
                        context.Result = new ContentResult { Content = $"You may only perform this action every 5secs.", ContentType = "text/plain" };
                        context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    }
                }
                else
                {
                    context.Result = new ContentResult { Content = $"Please provide Authorization Key", ContentType = "text/plain" };
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
            }
            else
            {

                context.Result = new ContentResult { Content = $"Please provide Authorization Key", ContentType = "text/plain" };
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }
    }
}
