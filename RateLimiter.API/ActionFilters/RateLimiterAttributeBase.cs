using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RateLimiter.Core;
using RateLimiter.Core.Rules;

namespace RateLimiter.API.ActionFilters
{
    public class RateLimiterAttributeBase : ActionFilterAttribute
    {
        protected IRule Rule { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue("AuthToken", out object authToken))
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            try
            {
                if (!Rule.AllowExecution((string) authToken))
                {
                    context.Result = new ContentResult
                    {
                        Content = Rule.GetNotAllowedReason((string) authToken)
                    };

                    context.HttpContext.Response.StatusCode = (int) HttpStatusCode.TooManyRequests;
                }
            }
            catch (Exception ex)
            {
                context.Result = new ContentResult
                {
                    Content = ex.Message
                };

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}