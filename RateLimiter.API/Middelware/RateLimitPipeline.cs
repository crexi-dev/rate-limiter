using Microsoft.AspNetCore.Mvc.Controllers;
using RateLimiter.Enums;
using System.Collections.Concurrent;
using System.Net;

namespace RateLimiter.API.Middelware
{
    public class RateLimitPipeline 
    {
        private readonly RequestDelegate _next;
        static readonly ConcurrentDictionary<string, DateTime?> ApiCallsInMemory = new();
        public RateLimitPipeline(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var actioncontroller = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (actioncontroller is null)
            {
                await _next(context);
                return;
            }

            var apiDecorator = (RateLimitingAttribute)actioncontroller.MethodInfo.GetCustomAttributes(true).SingleOrDefault(w => w.GetType() == typeof(RateLimitingAttribute));

            if (apiDecorator is null)
            {
                await _next(context);
                return;
            }

            string key = GetCurrentClientKey(apiDecorator, context);

            var validateOtherCalls = GetPreviousApiCallByKey(key);
            if (validateOtherCalls != null)
            {

                if (DateTime.Now < validateOtherCalls.Value.AddSeconds(5))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    return;
                }
            }
                                        
            UpdateApiCallFor(key);

            await _next(context);
        }

      
        private void UpdateApiCallFor(string key)
        {
            ApiCallsInMemory.TryRemove(key, out _);
            ApiCallsInMemory.TryAdd(key, DateTime.Now);
        }

        private DateTime? GetPreviousApiCallByKey(string key)
        {
            ApiCallsInMemory.TryGetValue(key, out DateTime? value);
            return value;
        }

        private static string GetCurrentClientKey(RateLimitingAttribute apiDecorator, HttpContext context)
        {
            var keys = new List<string>
            {
                context.Request.Path
            };

            if (apiDecorator.Restriction == RestrictionTypeEnum.IpAddress)
                keys.Add(GetClientByIp(context));

            if (apiDecorator.Restriction == RestrictionTypeEnum.PerUser)
                keys.Add(GetClientByUser(context));

            if (apiDecorator.Restriction == RestrictionTypeEnum.PerApiKey)
                keys.Add(GetClientByKey(context));

           

            return string.Join('_', keys);
        }

        private static string GetClientByKey(HttpContext context)
        {
            return "47267eb1-f1e6-4579-ad60-495194e9d70e";
        }

        private static string GetClientByUser(HttpContext context)
        {
            return "Victor";
        }

        private static string? GetClientByIp(HttpContext context)
        {
 
            return context?.Connection?.RemoteIpAddress?.ToString();
        }
    }
}