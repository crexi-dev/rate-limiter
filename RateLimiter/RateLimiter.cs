using RateLimiter.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class RateLimiter
    {
        private ConcurrentDictionary<string, List<Request>>? keyValues;

        public RateLimiter()
        {
            keyValues = new ConcurrentDictionary<string, List<Request>>();
        }

        [LimitStrategy(LimitStrategy.Header)]
        public bool ValidateRateLimit_Header(Request request)
        {
            return GuardRules(request, GetAttributeStrat("ValidateRateLimit_Header"));
        }

        [LimitStrategy(LimitStrategy.Endpoint)]
        public bool ValidateRateLimit_Endpoint(Request request)
        {
            return GuardRules(request, GetAttributeStrat("ValidateRateLimit_Endpoint"));
        }

        [LimitStrategy(LimitStrategy.Header)]
        [LimitStrategy(LimitStrategy.Endpoint)]
        public bool ValidateRateLimit_Multiple(Request request)
        {
            return GuardRules(request, GetAttributeStrat("ValidateRateLimit_Multiple"));
        }

        private bool GuardRules(Request request, List<LimitStrategy> strategy)
        {
            // Generate Key from request
            var key = GenerateKey(request, strategy);

            if (RateLimiterConfiguration.BlockList.Contains(key))
            {
                throw new Exception("Nope");
            }

            // Get key in keyValues
            if (keyValues == null)
            {
                keyValues = new ConcurrentDictionary<string, List<Request>>();
            }
            if (keyValues.ContainsKey(key))
            {
                // Iterate over rules
                var rules = RateLimiterConfiguration.Rules;
                var requests = keyValues[key];

                foreach (var rule in rules)
                {
                    ValidateRule(request, requests, rule, strategy);
                }

                requests.Add(request);
                keyValues.Remove(key, out _);
                return keyValues.TryAdd(key, requests);
            }
            else
            {
                return keyValues.TryAdd(key, new List<Request> { request });
            }
        }

        private void ValidateRule(Request request, List<Request> oldRequests, RateLimitRule rule, List<LimitStrategy> strategies)
        {
            foreach (var strategy in strategies)
            {
                // Determine matching strategy
                if (strategy == rule.Strategy)
                {
                    // Extract 'what to guard' based on request key
                    var guard = ExtractKey(request, strategy);
                    if (guard == rule.Guard)
                    {
                        // Determine which policy to use
                        if (rule.Policy == LimitPolicy.RequestsPerTimeSpan)
                        {
                            RateLimitEngine.RequestsPerTimeSpan(oldRequests, rule);
                        }
                        else if (rule.Policy == LimitPolicy.TimeSpanSinceLastRequest)
                        {
                            RateLimitEngine.TimeSpanSinceLastRequest(request, oldRequests, rule);
                        }
                    }
                }
            }
        }

        private static string GenerateKey(Request request, List<LimitStrategy> strategy)
        {
            var key = string.Empty;
            foreach (var item in strategy)
            {
                switch (item)
                {
                    case LimitStrategy.None:
                        key += $"{request.Key}-{request.IpAddress}-{request.Destination}";
                        break;
                    case LimitStrategy.Header:
                        if (RateLimiterConfiguration.AllowList.Contains(request.Key))
                        {
                            // Do something
                        }
                        key += $"Header-{request.Key}";
                        break;
                    case LimitStrategy.Endpoint:
                        key += $"Endpoint-{request.Destination}";
                        break;
                    case LimitStrategy.IP:
                        key += $"IP-{request.IpAddress}";
                        break;
                    default:
                        break;
                }
            }
            return key;
        }

        private static string ExtractKey(Request request, LimitStrategy strategy)
        {
            var key = string.Empty;
            switch (strategy)
            {
                case LimitStrategy.None:
                    key = $"{request.Key}-{request.IpAddress}-{request.Destination}";
                    break;
                case LimitStrategy.Header:
                    key = string.IsNullOrWhiteSpace(request.Key) ? "" : request.Key.Split('-')[2];
                    break;
                case LimitStrategy.Endpoint:
                    key = request.Destination;
                    break;
                case LimitStrategy.IP:
                    key = request.IpAddress;
                    break;
                default:
                    break;
            }
            return key;
        }

        private static List<LimitStrategy> GetAttributeStrat(string method)
        {
            return typeof(RateLimiter)
                                .GetMethod(method)
                                .GetCustomAttributes(typeof(LimitStrategyAttribute), false)
                                .Select(x => (LimitStrategy)x.GetType().GetProperty("Strategy").GetValue(x, null))
                                .ToList();
        }
    }
}
