using System;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Concurrent;
using System.Net;
using RateLimiter.Services;

namespace RateLimiter.Middleware
{
    public class RateLimitMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IRateLimitRules _iRateLimitRules;
        private readonly ILogApiHitCountService _iLogApiHitCountService;
        static readonly ConcurrentDictionary<string, DateTime?> ApiCallsInMemory = new();

        public RateLimitMiddleware(RequestDelegate next, IRateLimitRules iRateLimitRules, ILogApiHitCountService iLogApiHitCountService)
        {
            _next = next;
            _iRateLimitRules = iRateLimitRules;
            _iLogApiHitCountService = iLogApiHitCountService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var rateLimiterAttribute = endpoint?.Metadata.GetMetadata<RateLimitAttribute>();

            if (rateLimiterAttribute is null)
            {
                await _next(context);
                return;
            }

            if (rateLimiterAttribute is null)
            {
                await _next(context);
                return;
            }

            string key = GetCurrentClientKey(rateLimiterAttribute, context);

            var apiHitLogs = await _iLogApiHitCountService.GetApiCounts(key);

            var apiCallDateTime = GetApiCallDateTimeByKey(key);

            if (apiCallDateTime != null && apiHitLogs!= null)
            {
                apiHitLogs.NumberOfRequests++;
                
                await _iLogApiHitCountService.SaveApiCounts(key, apiHitLogs);

                // Check if the request is valid 
                if (_iRateLimitRules.IsValidRequestByKey(apiCallDateTime,key, rateLimiterAttribute.MaxRequests, apiHitLogs.NumberOfRequests))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    return;
                }

                UpdateApiCallFor(key);

                await _next(context);
            }
        }


            private void UpdateApiCallFor(string key)
            {
                ApiCallsInMemory.TryRemove(key, out _);
                ApiCallsInMemory.TryAdd(key, DateTime.Now);
            }

            private DateTime? GetApiCallDateTimeByKey(string key)
            {
                ApiCallsInMemory.TryGetValue(key, out DateTime? value);
                return value;
            }

            
            private static string GetCurrentClientKey(RateLimitAttribute apiAttribute, HttpContext context)
            {
                
                string key = context.Request.Path;

                if (apiAttribute.RuleType == RuleTypeEnum.IpAddress)
                    key = GetClientIpAddress(context);

                return key;
            }

            
            private static string GetClientIpAddress(HttpContext context)
            {
                return context.Connection.RemoteIpAddress.ToString();
            }
        }
    }


