using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using RateLimiter.Cache;
using RateLimiter.Rules;
using RateLimiter.Rules.Settings;

namespace RateLimiter
{
    public class Endpoint
    {
        private readonly ICacheService _cache;
        private readonly RequestPerTimespanSettings _requestPerTimespanSettings;
        private readonly TimespanSinceLastCallSettings _timespanSinceLastCallSettings;
        private readonly Dictionary<string, Resource> _resources = new();

        public Endpoint(IConfiguration configuration, ICacheService cache)
        {
            _cache = cache;

            // set rule settings
            _requestPerTimespanSettings =
                configuration.GetSection("RequestPerTimespan").Get<RequestPerTimespanSettings>();
            _timespanSinceLastCallSettings = configuration.GetSection("TimespanPassedSinceLastCall")
                .Get<TimespanSinceLastCallSettings>();
        }

        public void AddResource(Resource resource)
        {
            _resources[resource.Path] = resource;
        }

        public void AcceptRequest(UserRequest request, string path)
        {
            if (!_resources.ContainsKey(path))
                throw new Exception("resource not found by specified path");

            var resource = _resources[path];
            var rules = resource.GetRules().ToList();

            switch (request.Token.Region)
            {
                case Region.US:
                    rules.Add(new RequestPerTimespanRule(_requestPerTimespanSettings));
                    break;
                case Region.EU:
                    rules.Add(new TimespanSinceLastCallRule(_timespanSinceLastCallSettings));
                    break;
            }

            request.RequestTime = DateTime.Now;

            var accessGranted = CheckAccess(request, rules);

            request.State = accessGranted ? RequestState.Success : RequestState.AccessDenied;
        }

        private bool CheckAccess(UserRequest request, List<IRule> rules)
        {
            var validCacheInfo = new Dictionary<IRule, CacheEntry>();

            var key = request.Token.UserId.ToString();

            var cacheEntry = _cache.Get<CacheEntry>(key) ?? new();

            foreach (var rule in rules)
            {
                if (!rule.IsValid(request, cacheEntry))
                    return false; // this call is not valid, so we return and not even cache it

                validCacheInfo.Add(rule, cacheEntry);
            }

            rules.ForEach(r => _cache.Set(key, validCacheInfo[r]));

            return true;
        }
    }
}