using NSubstitute;
using RateLimiter.Common;
using RateLimiter.Rules;
using Shouldly;
using Xunit;

namespace RateLimiter.Tests.Rules
{
    public class RegionBaseRuleShould
    {
        private IRegionBasedRuleFactory regionBasedRuleFactory;
        private RegionBasedRule regionBasedRule;

        public RegionBaseRuleShould()
        {
            var apiClient = Substitute.For<IApiClient>();
            regionBasedRuleFactory = Substitute.For<IRegionBasedRuleFactory>();
            regionBasedRule = new RegionBasedRule(apiClient, regionBasedRuleFactory);
        }

        [Fact]
        public void Return_True_On_Rule()
        {
            var rule = Substitute.For<IRateLimiterRule>();
            rule.Validate().Returns(true);
            regionBasedRuleFactory.GetRateLimiterRule(Arg.Any<string>())
                .Returns(rule);

            var result = regionBasedRule.Validate();
            result.ShouldBeTrue();
        }

        [Fact]
        public void Return_False_On_Rule()
        {
            var rule = Substitute.For<IRateLimiterRule>();
            rule.Validate().Returns(false);
            regionBasedRuleFactory.GetRateLimiterRule(Arg.Any<string>())
                .Returns(rule);

            var result = regionBasedRule.Validate();
            result.ShouldBeFalse();
        }
    }
}
