using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using RateLimiter.BL;
using RateLimiter.BL.ServicesInterfaces;
using RateLimiter.Data;
using RateLimiter.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Filters
{

    public class RateLimiterFilter : IAsyncActionFilter
    {

        private readonly RateLimiterTypeEnum _type;
        private readonly IRaceLimiterService _raceLimiterService;

        public RateLimiterFilter(IRaceLimiterService raceLimiterService)
        {
            _raceLimiterService = raceLimiterService;
        }

        public RateLimiterFilter(IRaceLimiterService raceLimiterService, RateLimiterTypeEnum type)
        {
            _raceLimiterService = raceLimiterService;
            _type = type;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
        {
            var requestInfo = actionExecutingContext.HttpContext.Items["RequestInfo"] as Request;
            if (requestInfo == null)
            {
                return;
            }

            var limitersResult = await _raceLimiterService.ProcessLimiters(_type, requestInfo);
            if (limitersResult == false)
            {
                actionExecutingContext.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                actionExecutingContext.Result = new ContentResult
                {
                    Content = "Rate limit"
                };
                return;
            }

            await next();
        }
    }
}
