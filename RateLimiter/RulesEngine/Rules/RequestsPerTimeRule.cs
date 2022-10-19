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
    public class RequestsPerTimeRule : IRateLimiterRule
    {
        private int requestLimit;
        private int requestTimeSpan;
        private IStorage storageService;

        public RequestsPerTimeRule(IStorage storageService, IConfiguration configuration)
        {
            this.storageService = storageService;
            requestLimit = int.Parse(configuration["requestLimit"]);
            requestTimeSpan = int.Parse(configuration["requestTimeSpan"]);
        }

        public bool IsEnabled(ClientRequest request)
        {
            return ClientLocations.US.Equals(request.ClientLocation);
        }

        public bool Validate(ClientRequest request)
        {
            var lastRequest = storageService.GetToken(request.ClientToken.Ip.ToString());

            if (lastRequest == null)
            {
                storageService.SetToken(request.ClientToken.Ip.ToString(), new ClientRequestStorage { LastRequest = request.RequestTime, RequestCount = 1 });
                return true;
            }

            var result = lastRequest.LastRequest.AddMilliseconds(requestTimeSpan) < request.RequestTime;
            if (result)
            {
                lastRequest.LastRequest = request.RequestTime;
                lastRequest.RequestCount = 1;
                storageService.SetToken(request.ClientToken.Ip.ToString(), lastRequest);
                return true;
            }
            if (++lastRequest.RequestCount <= requestLimit)
            {
                storageService.SetToken(request.ClientToken.Ip.ToString(), lastRequest);
                return true;
            }

            return false;
        }
    }
}
