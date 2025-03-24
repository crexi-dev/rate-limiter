using RateLimiter.Data;
using RateLimiter.Helper;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Response;
using RateLimiter.Model.Rule.Config;

namespace RateLimiter.Rule.Validator
{
    public class ValidatorIpBlackList : ValidatorBase
    {
        private readonly RuleConfigIpBlackList Config;

        public ValidatorIpBlackList(IRepositoryRateLimiter repo, RuleConfigIpBlackList config)
            : base(repo, config.Rule)
        {
            Config = config;
        }

        public override ResponseGeneral Validate(ResourceRequest request)
        {
            var isBlackListed = (Config?.IpAddresses?.Contains(request.IpAddress) ?? false);

            return isBlackListed ?
                HelperResponse.GetFailure<ResponseGeneral>("Resource access restricted.") :
                HelperResponse.GetSuccess<ResponseGeneral>();
        }
    }
}
