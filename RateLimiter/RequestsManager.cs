using System.Collections.Generic;
using RateLimiter.Models;

namespace RateLimiter
{
    public class RequestsManager
    {
        private readonly RulesManager _rulesManager;
        private List<RequestModel> _requests = new();

        public RequestsManager()
        {
            _rulesManager = new RulesManager();
        }

        public bool ValidateRequest(RequestModel request, bool bothRules)
        {
            if (!_rulesManager.CheckRules(request, ref _requests, bothRules)) return false;

            _requests.Add(request);
            
            return true;
        }
    }
}