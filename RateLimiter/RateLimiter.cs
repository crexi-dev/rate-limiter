using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Model;
namespace RateLimiter
{
    /// <summary>
    /// Used to denote Result AuthorizeRequest
    /// </summary>
    public enum LimitResult
    {
        Success,
        Failure
    }
    /// <summary>
    /// Used to evaluate RateLimitRule to control access to API calls
    /// </summary>
    public class RateLimiter : IRateLimiter
    {
        #region Constructor
        public RateLimiter(List<RateLimitRule> rules, TimeSpan requestQueuelifespan)
        {
            Rules = rules;
            RequestQueue = new List<Request>();
            RequestQueueLifespan = requestQueuelifespan;
        }
        #endregion
        #region Properties
        private TimeSpan RequestQueueLifespan { get; set; }
        private List<Request> RequestQueue { get; set; }
        private List<RateLimitRule> Rules {get; set; }
        #endregion
        #region Methods
        public LimitResult AuthorizeRequest(Request request)
        {
            //ClearExpiredRequests();
            foreach (RateLimitRule rule in Rules)
            {
                //immediately return upon failure
                if (EvaluateRequest(request, rule) == LimitResult.Failure)
                    return LimitResult.Failure;
            }
            RequestQueue.Add(request);
            return LimitResult.Success;
        }
        private LimitResult EvaluateRequest(Request request, RateLimitRule rule)
        {
            //Empty Queue is automatic success
            if (RequestQueue.Count == 0) return LimitResult.Success;
            //filter tokens with Ages less than the rule duration 
            var activeRequests = RequestQueue.Where(request => request.Token.TokenAge() < rule.Duration).ToList();
            switch (rule.RuleType)
            {
                case RateLimitRuleType.LevelLimit:
                    return LevelLimitRequest(activeRequests, request, rule);
                case RateLimitRuleType.RegionLimit:
                    return RegionLimitRequest(activeRequests, request, rule);
                case RateLimitRuleType.RegionSpecificLimit:
                    return RegionSpecificLimitRequest(activeRequests, request, rule);
                case RateLimitRuleType.TokenLimit:
                    return TokenLimitRequest(activeRequests, request, rule);
                case RateLimitRuleType.LevelSpecificLimit:
                    return LevelSpecificLimitRequest(activeRequests, request, rule);
                default:
                    return DefaultLimitRequest(activeRequests, request, rule);
            }
        }

        private LimitResult DefaultLimitRequest(List<Request> activeRequests, Request request, RateLimitRule rule)
        {
            return (activeRequests.Count() >= rule.Limit) ? LimitResult.Failure : LimitResult.Success;
        }
        private LimitResult LevelLimitRequest(List<Request> activeRequests, Request request, RateLimitRule rule)
        {
            var count = activeRequests.Where(
                r => r.Token.Level == request.Token.Level).Count();
            return (count >= rule.Limit) ? LimitResult.Failure : LimitResult.Success;
        }
        private LimitResult LevelSpecificLimitRequest(List<Request> activeRequests, Request request, RateLimitRule rule)
        {
            var count = activeRequests.Where(
                r => r.Token.Level == (PriorityLevel)rule.RuleDetail && r.Token.Level == request.Token.Level).Count();
            var result = (count >= rule.Limit) ? LimitResult.Failure : LimitResult.Success;
            return result;
        }
        private LimitResult RegionLimitRequest(List<Request> activeRequest, Request request, RateLimitRule rule)
        {
            var count = activeRequest.Where(
                r => r.Token.Region == request.Token.Region).Count();
            return (count >= rule.Limit) ? LimitResult.Failure : LimitResult.Success;
        }
        private LimitResult RegionSpecificLimitRequest(List<Request> activeRequests, Request request, RateLimitRule rule)
        {
            var count = activeRequests.Where(
                r => r.Token.Region == (Region)rule.RuleDetail && r.Token.Region == request.Token.Region).Count();
            return (count >= rule.Limit) ? LimitResult.Failure : LimitResult.Success;
        }
        private LimitResult TokenLimitRequest(List<Request> activeRequests, Request request, RateLimitRule rule)
        {
            var count = activeRequests.Where(r => r.Token.Token == request.Token.Token).Count();
            return (count >= rule.Limit) ? LimitResult.Failure : LimitResult.Success;
        }
        
        /// <summary>
        /// Used to continually clear old request based on RequestQueueLifespan 
        /// </summary>
        private void ClearExpiredRequests()
        {
            if (RequestQueue.Count > 0)
            {
                var latestexpiredRequest = RequestQueue.Where(
                x => x.Token.TokenAge() >= RequestQueueLifespan).Last();
                var lastIndex = RequestQueue.IndexOf(latestexpiredRequest);
                RequestQueue.RemoveRange(0, lastIndex);
            }
        }
        #endregion
    }
}

