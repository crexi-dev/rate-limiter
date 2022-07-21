using Microsoft.AspNetCore.Http;
using RateLimiter.LocationService;
using RateLimiter.Model;
using RateLimiter.RulesEngine.Interfaces;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace RateLimiter.Middlewares
{
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate next;

        public RateLimiterMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IRuleEngine engine, ILocationService locationService)
        {
            var request = context.Request.Host.Host;
            var ip = context.Connection.RemoteIpAddress;
            var region = locationService.GetRegionFromIp(ip);
            var claimsIdentity = context.User.Identity as ClaimsIdentity;
            
            var userRequest = new UserRequest(new Token(ip), region); 
            if (engine.ProcessRules(userRequest))
            {
                await next(context);
                return;
            }
            await SendRateLimiterException(context);
        }

        private async Task SendRateLimiterException(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new
                {
                    StatusCode = (int)HttpStatusCode.TooManyRequests,
                    Message = "Rate limiter blocked request. Too many requests"
                }));
        }
    }
}
