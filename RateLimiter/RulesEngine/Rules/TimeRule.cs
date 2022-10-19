using Microsoft.Extensions.Configuration;
using RateLimiter.DataAccess;
using RateLimiter.Model;
using RateLimiter.RulesEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.RulesEngine.Rules
{
    public class TimeRule : IRateLimiterRule
    {
        private readonly IStorage storageService;
        private readonly int timeBetweenRequestsInMs;

        public TimeRule(IStorage storageService, IConfiguration configuration)
        {
            this.storageService = storageService;
            timeBetweenRequestsInMs = int.Parse(configuration["betweenRequestsTimeSpan"]);
        }

        public bool IsEnabled(ClientRequest request)
        {
            return ClientLocations.EU.Equals(request.ClientLocation);
        }

        public bool Validate(ClientRequest request)
        {
            var lastRequest = storageService.GetToken(request.ClientToken.Ip.ToString());

            if (lastRequest == null)
            {
                storageService.SetToken(request.ClientToken.Ip.ToString(), new ClientRequestStorage { LastRequest = request.RequestTime });
                return true;
            }

            var result = request.RequestTime - lastRequest.LastRequest > TimeSpan.FromMilliseconds(timeBetweenRequestsInMs);

            if (result)
            {
                lastRequest.LastRequest = request.RequestTime;
                storageService.SetToken(request.ClientToken.Ip.ToString(), lastRequest);
            }

            return result;
        }
    }
}
