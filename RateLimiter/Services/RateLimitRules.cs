using System;
using System.Data;
using System.Diagnostics.Metrics;
using System.Net;
using System.Numerics;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using RateLimiter.Services;

namespace RateLimiter.Middleware
{
    public class RateLimitRules : IRateLimitRules
    {
        private readonly ILogger<IRateLimitRules> _logger;

        public RateLimitRules(ILogger<IRateLimitRules> logger)
        {
            _logger = logger;

        }

        public bool IsValidRequestByKey(DateTime? apiCallDateTime, string key, int maxrequests, int hitCount)
        {
            if (apiCallDateTime != null)
            {
                switch (GetRegionByKey(key))
                {
                    case "USA":
                        if (DateTime.Now < apiCallDateTime.Value.AddSeconds(5) && hitCount >= maxrequests)
                        {
                            _logger.LogInformation("Request reached max limit for USA region");
                            return false;
                        }
                        break;
                    case "EUROPE":
                        if (DateTime.Now < apiCallDateTime.Value.AddSeconds(15) && hitCount >= maxrequests)
                        {
                            _logger.LogInformation("Request reached max limit for Europe region");
                            return false;
                        }
                        break;
                }
            }
            return false;
        }
        public bool IsValidRequestByClientToken(DateTime? dateTime, string token, int maxrequests, int hitCount)
        {
            // rules can also be customized based on the client token as well
            // Example: Check for token expiry 

            throw new NotImplementedException();
        }
        private string GetRegionByKey(string key)
        {
            if (key.StartsWith("IPAddressOFUSA"))
            {
                return "USA";
            }
            else if (key.StartsWith("IPAddressOFEUR"))
            {
                return "EUROPE";
            }
            return string.Empty;
        }
    }
}

