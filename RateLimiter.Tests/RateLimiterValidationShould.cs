using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using Shouldly;
using RateLimiter.Rules;

namespace RateLimiter.Tests
{
    public class RateLimiterValidationShould
    {
        private readonly RateLimiterValidation validation;
        private IRateLimiterRule rule1;
        private IRateLimiterRule rule2;
        private IRateLimiterRule rule3;
        private readonly List<IRateLimiterRule> rules;

        public RateLimiterValidationShould()
        {
            rule1 = Substitute.For<IRateLimiterRule>();
            rule2 = Substitute.For<IRateLimiterRule>();
            rule3 = Substitute.For<IRateLimiterRule>();
            rules = new List<IRateLimiterRule> { rule1, rule2, rule3 };
            validation = new RateLimiterValidation();
        }

        [Fact]
        public void Return_True_On_All_Tests_Passing()
        {
            rule1.IsValid().Returns(true);
            rule2.IsValid().Returns(true);
            rule3.IsValid().Returns(true);
            var result = validation.IsRequestValid(rules);
            result.ShouldBeTrue();
        }

        [Fact]
        public void Return_False_On_One_Test_Failing()
        {
            rule1.IsValid().Returns(true);
            rule2.IsValid().Returns(false);
            rule3.IsValid().Returns(true);
            var result = validation.IsRequestValid(rules);
            result.ShouldBeFalse();
        }
    }
}
