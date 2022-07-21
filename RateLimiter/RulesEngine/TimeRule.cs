using Microsoft.Extensions.Configuration;
using RateLimiter.DataAccess;
using RateLimiter.Model;
using RateLimiter.RulesEngine.Interfaces;
using System;

namespace RateLimiter.RulesEngine
{
    internal class TimeRule : IRateLimiterRule
    {
        private readonly IStorageService storageService;
        private readonly int timeBetweenRequestsInMs;
        private const string TimeBetweenRequestsInMsVar = "timeBetweenRequestsInMs";

        public TimeRule(IStorageService storageService, IConfiguration configuration)
        {
            this.storageService = storageService;
            timeBetweenRequestsInMs = int.Parse(configuration[TimeBetweenRequestsInMsVar]);
        }

        public bool IsEnabled(UserRequest request)
        {
            return Regions.EU.Equals(request.Region);
        }

        public bool Validate(UserRequest request)
        {
            var lastRequest = storageService.GetToken(request.Token.Ip.ToString());

            if (lastRequest == null)
            {
                storageService.SetToken(request.Token.Ip.ToString(), new UserRequestCache { LastRequest = request.RequestTime });
                return true;
            }

            var result = request.RequestTime - lastRequest.LastRequest > TimeSpan.FromMilliseconds(timeBetweenRequestsInMs);

            if (result)
            {
                lastRequest.LastRequest = request.RequestTime;
                storageService.SetToken(request.Token.Ip.ToString(), lastRequest);
            }            

            return result;
        }
    }
}
