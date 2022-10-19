using Microsoft.AspNetCore.Http;
using RateLimiter.Model;
using RateLimiter.RulesEngine.Interfaces;
using System;
using System.Net;
using System.Net.Http;
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

        public async Task Invoke(HttpContext context, IRuleEngine engine)
        {
            var request = context.Request.Host.Host;
            var ip = context.Connection.RemoteIpAddress;
            var Location = ClientLocations.US;
            var claimsIdentity = context.User.Identity as ClaimsIdentity;

            var ClientRequest = new ClientRequest(new ClientToken(ip), Location, DateTime.UtcNow);
            if (engine.ProcessRules(ClientRequest))
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