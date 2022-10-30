using RateLimiter.Models;
using RateLimiter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.Rules
{
    internal class NumberRequestsPerTimeRule : IBaseRule
    {
        public RateLimiterType RuleType => RateLimiterType.NumberRequestsPerTime;

        public bool Validate(RequestAttributeDataModel limitationsData, IList<RequestsHistoryModel> requestsHistoryData)
        {
            var dateTime = DateTime.UtcNow;
            var regionsData = requestsHistoryData ?? new List<RequestsHistoryModel>();

            if (limitationsData.RequestedRegions is not null && limitationsData.RequestedRegions.Any())
            {
                regionsData = regionsData.Where(e => limitationsData.RequestedRegions.Contains(e.RegionName)).ToList();
            }

            var processedRequests = regionsData.Count(e => dateTime - e.DateTime <= limitationsData.RequestedTimeWindow);            

            return !(processedRequests > limitationsData.RequestedMaxRequests);
        }
    }
}
