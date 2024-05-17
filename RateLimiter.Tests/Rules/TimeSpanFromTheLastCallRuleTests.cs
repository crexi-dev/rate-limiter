using NUnit.Framework;
using RateLimiter.Models;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class TimeSpanFromTheLastCallRuleTest
    {
        [TestCase(100, 200, ExpectedResult = true)]
        [TestCase(200,100, ExpectedResult = false)]
        public bool Validate(int expectedTimeSpan,  int actualTimeSpan)
        {
            // Arrange
            var rule = new TimeSpanFromTheLastCallRule(expectedTimeSpan);

            // Act
            var result = rule.Validate(new Request());

            // Assert
            return result;
        }
    }
}
