using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using RateLimiter.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Filters
{

    public class LogRequestsFilter : IAsyncActionFilter
    {
        private readonly IRequestRepository _requestRepository;


        public LogRequestsFilter(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
        {
            var context = actionExecutingContext.HttpContext;

            var request = new Data.Request()
            {
                AccessToken = context.Request.Cookies["AuthToken"],
                Ip = context.Request.Host.Value,
                Resource = context.Request.Path
            };
            context.Items.Add("RequestInfo", request);

            await next();

            await _requestRepository.Add(request);
        }
    }
}
