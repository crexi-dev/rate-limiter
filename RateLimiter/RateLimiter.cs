using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RateLimiter
{
    public class RateLimiter
    {
        private readonly Dictionary<string, List<IRateLimitStrategy>> _strategies = new();
        private readonly IRequestRepository _requestRepository;

        public RateLimiter(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }
        public void AddLimitationRule(string resource, params IRateLimitStrategy[] rules)
        {
            _strategies.Add(resource, rules.ToList());
        }
        public bool TryMakeRequest(string username, string resource, string? region ="US" )
        {
            if (!_strategies.TryGetValue(resource, out var strategies))
                return true;

            foreach (var strategy in strategies)
            {
                if (!strategy.IsRequestAllowed(username, resource, _requestRepository,region))
                {
                    return false;
                }
            }

            _requestRepository.AddRequest(username, resource, DateTime.UtcNow, region);

            return true;
        }
    }
}
