using RateLimiter.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class RateLimiter
    {
        private int _limitRequests;
        private int _limitsRequestsPerResource;
        private TimeSpan? _timeLimit;
        private TimeSpan? _timeLimitPerResource;
        private TimeSpan? _differenceBetweenRequests;
        private readonly IList<ResourceRuleModel> _resourceRules;
        private IList<ResourceRequestModel> _requestStorage;

        /// <summary>
        /// RateLimiter
        /// </summary>
        /// <param name="parameters">parameters for Rate limiter</param>
        /// <param name="resourceRules">Rules for each resource</param>
        /// <param name="requestStorage"></param>
        public RateLimiter(
            RateLimiterParameters parameters,
            IList<ResourceRuleModel> resourceRules,
            IList<ResourceRequestModel> requestStorage)
        {
            _limitRequests = parameters.LimitRequests;
            _limitsRequestsPerResource = parameters.LimitsRequestsPerResource;
            _timeLimit = parameters.TimeLimit;
            _timeLimitPerResource = parameters.TimeLimitPerResource;
            _differenceBetweenRequests = parameters.DifferenceBetweenRequests;
            _resourceRules = resourceRules;
            _requestStorage = requestStorage;
        }
        public RateLimiter(IList<ResourceRequestModel> requestStorage) : this(
            new RateLimiterParameters
            {
                LimitRequests = 30,
                LimitsRequestsPerResource = 10,
                TimeLimit = new TimeSpan(0, 0, 60),
                TimeLimitPerResource = new TimeSpan(0, 0, 30),
                DifferenceBetweenRequests = null
            }, null, requestStorage)
        { }

        public bool IsResourceAllowed(string resourceName, string accessToken)
        {
            bool allow = true;
            int requestResourceLimit = 0;
            TimeSpan? timeResourceLimit = null;
            TimeSpan? differenceBetweenRequestsPerResource = null;

            if (_resourceRules != null && _resourceRules.Any(o => o.ResourceName
                                                                    .Equals(resourceName, StringComparison.InvariantCultureIgnoreCase)))
            {
                var resourceRule = _resourceRules.FirstOrDefault(o => o.ResourceName
                                                                        .Equals(resourceName, StringComparison.InvariantCultureIgnoreCase));
                requestResourceLimit = resourceRule.LimitRequests;
                timeResourceLimit = resourceRule.TimeLimit;
                differenceBetweenRequestsPerResource = resourceRule.DifferenceBetweenRequests;
            }
            else
            {
                if (_limitsRequestsPerResource > 0)
                {
                    requestResourceLimit = _limitsRequestsPerResource;
                }
                if (_timeLimitPerResource.HasValue)
                {
                    timeResourceLimit = _timeLimitPerResource.Value;
                }
            }

            if (_requestStorage == null)
            {
                _requestStorage = new List<ResourceRequestModel>();
            }

            var utcNowDate = DateTime.UtcNow;

            if (differenceBetweenRequestsPerResource.HasValue)
            {
                if (differenceBetweenRequestsPerResource.HasValue)
                {
                    var lastRequest = _requestStorage
                                            .OrderByDescending(o => o.UtcDateRequest)
                                            .FirstOrDefault(o =>
                                                        o.ResourceName.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase)
                                                        &&
                                                        o.AccessToken.Equals(accessToken, StringComparison.InvariantCultureIgnoreCase));
                    if (lastRequest != null && utcNowDate - lastRequest.UtcDateRequest < differenceBetweenRequestsPerResource.Value)
                    {
                        allow = false;
                    }
                }
            }
            if (_differenceBetweenRequests.HasValue)
            {
                var lastRequest = _requestStorage
                                        .OrderByDescending(o => o.UtcDateRequest)
                                        .FirstOrDefault(o => o.AccessToken.Equals(accessToken, StringComparison.InvariantCultureIgnoreCase));
                if (lastRequest != null && utcNowDate - lastRequest.UtcDateRequest < _differenceBetweenRequests.Value)
                {
                    allow = false;
                }
            }

            _requestStorage.Add(new ResourceRequestModel
            {
                ResourceName = resourceName,
                AccessToken = accessToken,
                UtcDateRequest = utcNowDate
            });

            if (requestResourceLimit > 0 || timeResourceLimit.HasValue || differenceBetweenRequestsPerResource.HasValue)
            {
                var requests = _requestStorage
                                        .Where(o =>
                                                    o.ResourceName.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase)
                                                    &&
                                                    o.AccessToken.Equals(accessToken, StringComparison.InvariantCultureIgnoreCase)
                                                    &&
                                                    (
                                                        timeResourceLimit.HasValue
                                                        ? utcNowDate - o.UtcDateRequest <= timeResourceLimit.Value
                                                        : true
                                                    ))
                                        .OrderBy(o => o.UtcDateRequest)
                                        .ToList();
                if (requests.Count > requestResourceLimit)
                {
                    allow = false;
                }
            }
            if (_limitRequests > 0 || _timeLimit.HasValue)
            {

                var requests = _requestStorage
                                        .Where(o =>
                                                    o.AccessToken.Equals(accessToken, StringComparison.InvariantCultureIgnoreCase)
                                                    &&
                                                    (
                                                        _timeLimit.HasValue
                                                        ? utcNowDate - o.UtcDateRequest <= _timeLimit.Value
                                                        : true
                                                    ))
                                        .OrderBy(o => o.UtcDateRequest)
                                        .ToList();
                if (requests.Count > _limitRequests)
                {
                    allow = false;
                }
            }

            return allow;
        }
    }
}
