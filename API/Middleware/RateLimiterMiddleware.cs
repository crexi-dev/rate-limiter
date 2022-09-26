using API.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Middleware
{
    public enum ResultUpdateApiCall
    {
        ApiUpdated = 1,
        RequestBlock = 2,
    }

    public class RateLimiterMiddleware
    {
        private readonly IConfiguration _configur;
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;
        private readonly DataProvider dataProvider;

        public RateLimiterMiddleware(
            IConfiguration configur,
            RequestDelegate next,
            IMemoryCache memoryCache)
        {
            this._next = next;
            this._configur = configur;
            this.dataProvider = new DataProvider();
            this._configur.Bind("RateLimiter", dataProvider);
            this._memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var descriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (descriptor is null)
            {
                await _next(context);
                return;
            }

            string token = GetClientAccessToken(context);
            string endpointPath = context?.Request?.Path.Value;
            Resources resource = this.dataProvider.Resources?
                .Where(w => w.URL == endpointPath)
                .SingleOrDefault();

            string[] rulesArr = resource?.Rules?.Split(',');

            if (rulesArr != null && rulesArr.Any())
            {
                for (int i = 0; i < rulesArr.Length; i++)
                {
                    ResultUpdateApiCall result = this.UpdateApiCallFor(
                        token,
                        endpointPath,
                        this.dataProvider.Rules
                            .Where(w => w.Rule == rulesArr[i]).ToList());

                    if (result == ResultUpdateApiCall.RequestBlock)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                        await context.Response.WriteAsync("Blocked request");
                        break;
                    }
                }
            }

            await _next(context);
        }

        /// <summary>
        /// We store the time that a client made a call to the current API
        /// </summary>
        private ResultUpdateApiCall UpdateApiCallFor(string key, string endpoint, List<Rules> rules)
        {
            int currentReqCounter = 0;
            List<UserRequestInfo> requestList = this._memoryCache.Get<List<UserRequestInfo>>(key);

            if (requestList != null && requestList.Any())
            {
                foreach (var req in requestList)
                {
                    if (req.EndPoint == endpoint)
                    {
                        foreach (var rule in rules)
                        {
                            DateTime reqTime = req.Timestamp;
                            DateTime maxRoleTime = reqTime.AddSeconds(rule.Limit);

                            if (maxRoleTime >= DateTime.Now)
                            {
                                if (rule.MaxRequests <= currentReqCounter)
                                    return ResultUpdateApiCall.RequestBlock;
                                else
                                    currentReqCounter++;
                            }
                            else
                                return ResultUpdateApiCall.RequestBlock;
                        }
                    }
                }

                requestList.Add(new UserRequestInfo()
                {
                    EndPoint = endpoint,
                    Timestamp = DateTime.Now
                });

                this._memoryCache.Set(key, requestList, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromSeconds(86400),
                    Priority = CacheItemPriority.Normal
                });
            }
            else
            {
                requestList = new List<UserRequestInfo>
                {
                    new UserRequestInfo()
                    {
                        EndPoint = endpoint,
                        Timestamp = DateTime.Now
                    }
                };

                this._memoryCache.Set(key, requestList, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromSeconds(86400),
                    Priority = CacheItemPriority.Normal
                });
            }

            return ResultUpdateApiCall.ApiUpdated;
        }

        /// <summary>
        /// Returns the client's Access Token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetClientAccessToken(HttpContext context)
        {

            return context.Request.Headers["AppToken"];
        }

        private class UserRequestInfo
        {
            public DateTime Timestamp { get; set; }
            public string EndPoint { get; set; }
        }
    }
}
