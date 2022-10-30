using RateLimiter.Models;
using RateLimiter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.Rules
{
    internal class TimeBetweenTwoRequestsRule : IBaseRule
    {
        public RateLimiterType RuleType => RateLimiterType.TimeBetweenTwoRequests;

        public bool Validate(RequestAttributeDataModel limitationsData, IList<RequestsHistoryModel> requestsHistoryData)
        {
            var dateTime = DateTime.UtcNow;
            var regionsData = requestsHistoryData ?? new List<RequestsHistoryModel>();

            if (limitationsData.RequestedRegions is not null && limitationsData.RequestedRegions.Any())
            {
                regionsData = regionsData.Where(e => limitationsData.RequestedRegions.Contains(e.RegionName)).ToList();
            }

            var lastRequest = regionsData.OrderBy(e => e.DateTime).LastOrDefault();

            if (lastRequest is null)
            {
                return true;
            }

            return !(dateTime - lastRequest.DateTime <= limitationsData.RequestedTimeWindow);
        }
    }
}
