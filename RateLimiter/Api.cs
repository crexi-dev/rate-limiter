using System;
using RateLimiter.Models;

namespace RateLimiter
{
    public class Api
    {
        private readonly RequestsManager _requestsManager;

        public Api()
        {
            _requestsManager = new RequestsManager();
        }
        
        public string GetRequest(RequestModel request, bool bothRules = false)
        {
            return _requestsManager.ValidateRequest(request, bothRules) ? "Ok" : throw new Exception("BadRequest");
        }
    }
}