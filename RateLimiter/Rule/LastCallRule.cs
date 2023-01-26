using System;
using System.Linq;
using RateLimiter.Models;
using RateLimiter.Rule.Contracts;

namespace RateLimiter.Rule
{
    public class LastCallRule: IRule
    {
        public TimeSpan LastCall { get; set; }
        public bool ValidateRuleByStatistic(ClientStatistic statistic)
        {
            var lastRequest = statistic.Requests.OrderByDescending(x => x).ToList();
            return lastRequest.FirstOrDefault() - lastRequest.Skip(1).FirstOrDefault() >= LastCall;
        }
    }
}