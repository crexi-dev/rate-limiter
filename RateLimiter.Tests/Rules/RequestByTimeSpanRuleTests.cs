using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class RequestByTimeSpanRuleTest
{
    [TestCase(100, 10, 100, ExpectedResult = false)]
    [TestCase(120, 10, 100, ExpectedResult = true)]
    public bool Validate(int requestLimit, int timeSpan, int actualValueInTimeSpan)
    {
        // Arrange
        var rule = new RequestByTimeSpanRule(requestLimit, timeSpan);
        var request = new Request();
        // Act
        var result = rule.Validate(request);

        // Assert
        return result;
    }
}