using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.Rules
{
    public class RequestRateValidator : IRateLimiter
    {

        private static readonly Dictionary<string, RequestDetails> _requests = new Dictionary<string, RequestDetails>();
        private const int _MaxRequestCount = 5;

      
        public bool ApplyRule(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                if (_requests.TryGetValue(token, out RequestDetails requestDetails))
                {

                    //if (request.recordedTimeInterval.TotalMinutes > 1)
                    //    return false;
                    //else if (request.recordedTimeInterval.TotalMinutes < 1 && request.count >= _MaxRequestCount)
                    //    return false;
                    if (requestDetails.count > _MaxRequestCount)
                    {
                        return false;
                    }
                    else
                    {
                        requestDetails.count += 1;
                        return true;
                    }

                }
                else
                {
                    var request = new RequestDetails
                    {
                        count = 1,
                        recordedTimeInterval = new TimeSpan(0,1,0)
                    };
                    _requests.Add(token, request);
                    return true;
                }
            }
            return false;
        }
    }
}
