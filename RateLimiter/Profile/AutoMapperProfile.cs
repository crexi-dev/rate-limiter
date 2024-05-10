using RateLimiter.Models;

namespace RateLimiter.Profile
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RuleConfigurationModel, ReadRuleResponseModel>();
            CreateMap<ReadRuleResponseModel, RuleExecuteRequestModel>();
            CreateMap<RuleExecuteRequestModel, RateLimitRuleModel>();
        }
    }
}
