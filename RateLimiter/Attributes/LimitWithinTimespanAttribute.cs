using RateLimiter.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Attributes
{
    public class LimitWithinTimespanAttribute : RateLimiterAttribute
    {
        public uint AllowedRequestCount { get; set; }
        public uint PerMinutes { get; set; }

        public override async Task<RateLimiterResponse> ExecuteLimiterAsync(RateLimiterRequest currentRequest)
        {
            var allowedRequests = RequestService.GetAllowedRequests(currentRequest.ControllerName, currentRequest.ActionName, startDate: DateTime.Today);

            var latestRequestGroup = (from req in allowedRequests
                                      orderby req.RequestDate descending
                                      group req by req.GroupId into reqGroups
                                      select reqGroups).FirstOrDefault();

            if (latestRequestGroup == default)
                return RateLimiterResponse.Allow();

            var initRequest = latestRequestGroup.Last();
            var timePassed = DateTime.Now - initRequest.RequestDate;

            if (timePassed.TotalMinutes <= PerMinutes)
            {
                currentRequest.GroupId = initRequest.GroupId;

                if (latestRequestGroup.Count() >= AllowedRequestCount)
                    return RateLimiterResponse.Deny(111, $"{PerMinutes * 60 - timePassed.TotalSeconds} seconds must pass before another request is allowed");
            }
                
            return RateLimiterResponse.Allow();
        }
    }
}
