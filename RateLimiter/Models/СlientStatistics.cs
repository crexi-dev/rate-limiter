using RateLimiter.Consts;
using System;
using System.Collections.Generic;

namespace RateLimiter.Models
{
    public sealed class СlientStatistics
    {
        private History<DateTime> requests = new History<DateTime>(Constants.MaxRequestsCount);

        public СlientStatistics(string clientId, string resourceName)
        {
            ClientId = clientId;
            ResourceName = resourceName;
        }

        public string ClientId { get; set; }

        public string ResourceName { get; set; }

        public List<DateTime> RequestsHistory => requests.Items;

        public DateTime? LastRequest { get; set; }

        public DateTime? LastSuccessfulRequest { get; set; }

        public void Update(RateLimiterResult result)
        {
            var dateTime = DateTime.UtcNow;

            requests.Add(dateTime);

            LastRequest = dateTime;

            if (!result.IsRateLimited)
                LastSuccessfulRequest = dateTime;
        }
    }
}
