using NUnit.Framework;
using RateLimiter.Rules.RuleInfo;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class ReqionBasedRuleTests
    {
        [Test]
        public void Validate()
        {
            // Arrange
            var rule = new RegionBasedRule<TimeSpanFromTheLastCallInfo>("us", new TimeSpanFromTheLastCallRule(100));

            // Act
            var result = rule.Validate(
                new RegionBasedInfo<TimeSpanFromTheLastCallInfo>
                    { 
                        Region = "us", 
                        InnerRuleInfo = new TimeSpanFromTheLastCallInfo{ ActualTime = 120 } });

            // Assert
            Assert.That(result, Is.True);
        }
    }
}