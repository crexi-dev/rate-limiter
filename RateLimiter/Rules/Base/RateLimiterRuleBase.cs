using RateLimiter.Core;
using RateLimiter.Rules.Interfaces;
using System.Threading.Tasks;

namespace RateLimiter.Rules.Base
{
    public abstract class RateLimiterRuleBase : IRateLimiterRule
    {
        protected RateLimiterRuleBase() { }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string ViolationMessage { get; }
        public abstract Task<bool> IsRequestAllowedAsync(ClientRequestContext context);
        
        protected virtual string GenerateRequestKey(ClientRequestContext context)
        {
            return $"{context.Token}:{context.Endpoint}";
        }
    }
}
