using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using RateLimiter.Configuration.Options;
using RateLimiter.Exceptions;
using RateLimiter.Models;

namespace RateLimiter.Services
{
    public class LimiterStore
    {
        private readonly IList<LocationLimit> _limiterStore;
        private readonly LimiterOptions _limiterOptions;

        private const int InitialRequestsCount = 1;

        public LimiterStore(IOptions<LimiterOptions> limiterOptions)
        {
            _limiterOptions = limiterOptions.Value;
            _limiterStore = new List<LocationLimit>();
        }

        public void CheckLocationRequestCount(string locationName)
        {
            var locationSettings = _limiterOptions
                .LocationLimiters?
                .FirstOrDefault(_ => _.LocationName == locationName);

            if (locationSettings != null)
            {
                var existingLocationLimit = _limiterStore.FirstOrDefault(_ => _.LocationName == locationName);

                if (existingLocationLimit == null)
                {
                    _limiterStore.Add(new LocationLimit
                    {
                        LocationName = locationName,
                        ProceedRequestsCount = InitialRequestsCount,
                        FirstRequestTime = DateTime.UtcNow
                    });
                }
                else
                {
                    existingLocationLimit.ProceedRequestsCount++;
                    
                    var timeRange = DateTime.UtcNow - existingLocationLimit.FirstRequestTime;

                    if (timeRange > locationSettings.TimeRange)
                    {
                        _limiterStore.Remove(existingLocationLimit);
                        _limiterStore.Add(new LocationLimit
                        {
                            LocationName = locationName,
                            ProceedRequestsCount = InitialRequestsCount,
                            FirstRequestTime = DateTime.UtcNow
                        });
                    }
                    else if (existingLocationLimit.ProceedRequestsCount > locationSettings.AllowedRequestsCountPerTimeRange 
                             && timeRange < locationSettings.TimeRange)
                    {
                        throw new AllowedRequestsCountReachedException();
                    }
                }
            }
        }
    }
}