using Microsoft.Extensions.Configuration;
using RateLimiter.DataAccess;
using RateLimiter.Model;
using RateLimiter.RulesEngine.Interfaces;
using System;

namespace RateLimiter.RulesEngine
{
    internal class RequestsPerTimeRule : IRateLimiterRule
    {
        private readonly int maxRequests;
        private readonly int timeSpan;
        private readonly IStorageService storageService;
        private const string MaxRequests = "maxRequestInTs";
        private const string TimeSpanRequest = "timeRequestSpanInMs";


        public RequestsPerTimeRule(IStorageService storageService, IConfiguration configuration)
        {
            this.storageService = storageService;
            maxRequests = int.Parse(configuration[MaxRequests]);
            timeSpan = int.Parse(configuration[TimeSpanRequest]);
        }

        public bool IsEnabled(UserRequest request)
        {
            return Regions.US.Equals(request.Region);
        }

        public bool Validate(UserRequest request)
        {
            var lastRequest = storageService.GetToken(request.Token.Ip.ToString());

            if (lastRequest == null)
            {
                storageService.SetToken(request.Token.Ip.ToString(), new UserRequestCache { LastRequest = request.RequestTime, RequestCount = 1 });
                return true;
            }

            var result = lastRequest.LastRequest.AddMilliseconds(timeSpan) < request.RequestTime;
            if (result)
            {
                lastRequest.LastRequest = request.RequestTime;
                lastRequest.RequestCount = 1;
                storageService.SetToken(request.Token.Ip.ToString(), lastRequest);
                return true;
            }
            if (++lastRequest.RequestCount <= maxRequests)
            {
                storageService.SetToken(request.Token.Ip.ToString(), lastRequest);
                return true;
            }

            return false;
        }
    }
}
