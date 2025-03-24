using RateLimiter.Data;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Response;
using RateLimiter.Model.Rule;

namespace RateLimiter.Rule.Validator
{
    public abstract class ValidatorBase
    {
        protected readonly EnumRateLimitRule Rule;
        protected readonly IRepositoryRateLimiter Repo;

        public abstract ResponseGeneral Validate(ResourceRequest request);

        protected ValidatorBase(IRepositoryRateLimiter repo, EnumRateLimitRule rule) 
        { 
            Repo = repo;
            Rule = rule;            
        }
    }
}
