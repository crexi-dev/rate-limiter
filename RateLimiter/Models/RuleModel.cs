using System;
using Newtonsoft.Json;
using RateLimiter.Enums;

namespace RateLimiter.Models
{
    public class RuleModel
    {
        [JsonProperty("IntervalMs")]
        private int _interval;
        [JsonProperty("AllowedAmount")]
        private int _allowedAmount;

        public TimeSpan Interval => TimeSpan.FromMilliseconds(_interval);
        public int AllowedRequests => _allowedAmount;

        public LocationEnum Location { get; set; }
    }
}