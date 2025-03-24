using RateLimiter.Data;
using RateLimiter.Helper;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Response;
using RateLimiter.Model.Rule.Config;

namespace RateLimiter.Rule.Validator
{
    public class ValidatorEnabledResource : ValidatorBase
    {
        private readonly RuleConfigEnabledResource Config;
        public ValidatorEnabledResource(IRepositoryRateLimiter repo, RuleConfigEnabledResource config)
            : base(repo, config.Rule)
        {
            Config = config;
        }

        public override ResponseGeneral Validate(ResourceRequest request)
        {
            return (Config?.Enabled ?? false) ?
                HelperResponse.GetSuccess<ResponseGeneral>() :
                HelperResponse.GetFailure<ResponseGeneral>("Resource is disabled.");
        }
    }
}
