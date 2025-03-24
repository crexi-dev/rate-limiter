using RateLimiter.Data;
using RateLimiter.Helper;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Response;
using RateLimiter.Model.Rule.Config;

namespace RateLimiter.Rule.Validator
{
    public class ValidatorIpWhiteList : ValidatorBase
    {
        private readonly RuleConfigIpWhiteList Config;

        public ValidatorIpWhiteList(IRepositoryRateLimiter repo, RuleConfigIpWhiteList config) 
            : base(repo, config.Rule)
        {
            Config = config;
        }

        public override ResponseGeneral Validate(ResourceRequest request)
        {
            var isWhiteListed = (Config?.IpAddresses?.Contains(request.IpAddress) ?? false);

            return isWhiteListed ?
                HelperResponse.GetSuccess<ResponseGeneral>() :
                HelperResponse.GetFailure<ResponseGeneral>("Resource requires your IP to be whitelisted.");
        }
    }    
}
