using RateLimiter.Enums;
using RateLimiter.Models;
using RateLimiter.Rules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace RateLimiter.Services
{
    public class RateLimiterService
    {
        //We use concurrent dictionary to ensure thread safety.
        //In a real situation, I might put the rules and requests in a database.
        private readonly ConcurrentDictionary<string, List<RuleModel>> _rulesBySource;
        private ConcurrentDictionary<string, List<RequestModel>> _requestsBySourceAndUserId;


        public RateLimiterService(string rulesBySourceJsonString)
        {

            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter() 
                }
            };

            _rulesBySource = string.IsNullOrEmpty(rulesBySourceJsonString) ? new ConcurrentDictionary<string, List<RuleModel>>() :
                    JsonSerializer.Deserialize<ConcurrentDictionary<string, List<RuleModel>>>(rulesBySourceJsonString, options)
                    ?? new ConcurrentDictionary<string, List<RuleModel>>();

            _requestsBySourceAndUserId = new ConcurrentDictionary<string, List<RequestModel>>();

        }

        public RateLimiterService(ConcurrentDictionary<string, List<RuleModel>> rulesBySource)
        {
            _rulesBySource = rulesBySource;

            _requestsBySourceAndUserId = new ConcurrentDictionary<string, List<RequestModel>>();
        }

        private bool RuleRunner(RequestModel currentRequest, string requestKey, RuleModel rule)
        {
            if (!_requestsBySourceAndUserId.TryGetValue(requestKey, out List<RequestModel>? requests))
            {
                throw new ApplicationException($"Requests dont exist for key: {requestKey}.");
            }

            switch (rule.Type)
            {
                case RuleType.FixedWindow:
                    FixedWindowRateLimiterRule fixedWindowRateRule = new();
                    return fixedWindowRateRule.IsRequestAllowed(currentRequest, requests, rule);
                    
                case RuleType.SlidingWindow:
                    SlidingWindowRateLimiterRule slidingWindowRateRule = new();
                    return slidingWindowRateRule.IsRequestAllowed(currentRequest, requests, rule);
                default:
                    throw new ArgumentException("Invalid rule type");
            };
        }

        public bool ValidateRequest(RequestModel request, RuleModel defaultRule=null)
        {
            if(String.IsNullOrEmpty(request.Source) || String.IsNullOrEmpty(request.UserID)) return false;

            if (!_rulesBySource.ContainsKey(request.Source))
            {
                //If user doesn't provide a default rule and we can't find a rule for this source, we assume a mistake was made. 
                if(defaultRule == null)
                {
                    throw new ApplicationException($"Cannot find RateLimiterRule for {request.Source} and a default rule was not provided.");
                }

                _rulesBySource.GetOrAdd(request.Source, new List<RuleModel>() { defaultRule });
            }

            string requestKey = $"{request.Source}-{request.UserID}";

            _requestsBySourceAndUserId.GetOrAdd(requestKey, new List<RequestModel>() { request }).Add(request);

            //Check that the request passed all rules.
            foreach (var rule in _rulesBySource[request.Source])
            {
                if (!RuleRunner(request, requestKey, rule))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
