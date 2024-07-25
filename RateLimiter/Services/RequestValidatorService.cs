using RateLimiter.Interfaces;
using RateLimiter.Models;

namespace RateLimiter.Services
{
    public class RequestValidatorService : IRequestValidatorService
    {
        private IExtendableRuleFactory extendableRuleFactory;

        public RequestValidatorService(IExtendableRuleFactory extendableRuleFactory)
        {
            this.extendableRuleFactory = extendableRuleFactory;
        }

        public bool ValidateUserRequest(UserRequest request)
        {
            var rulesList = extendableRuleFactory.GetRulesByServiceType(request.requestedServiceType);
            var acquireRequest = false;

            foreach (var rule in rulesList)
            {
                acquireRequest = rule.Acquire(request.requestedServiceType, request.userToken, request.requestedDate);
            }

            return acquireRequest;
        }
    }
}
