using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RequestLimiter : IRequestLimiter
    {
        private readonly RequestDelegate _next;
        private readonly IRulesFactory _rules;
        private IContextHelper _contextHelper;
        private ICacheHelper _cacheHelper;

        public RequestLimiter(RequestDelegate next, IRulesFactory rules, IContextHelper contextHelper, ICacheHelper cacheHelper)
        {
            _next = next;
            _rules = rules;
            _contextHelper = contextHelper;
            _cacheHelper = cacheHelper;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var contextDto = _contextHelper.GetDto(context);
            var rule = _rules.GetRule(contextDto);
            //var path = context.Request.Path.ToString();

            var isAllowed = rule?.CheckLimit() ?? true;

            if (!isAllowed)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                return;
            }

            _cacheHelper.AddRequest(contextDto.Id);

            await _next(context);
        }
    }
}
