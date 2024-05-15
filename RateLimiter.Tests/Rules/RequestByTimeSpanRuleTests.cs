using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class RequestByTimeSpanRuleTest
{
    [TestCase(100, 120, ExpectedResult = false)]
    [TestCase(120, 100, ExpectedResult = true)]
    public bool Validate(int requestLimit, int requestNumber)
    {
        // Arrange
        var rule = new RequestByTimeSpanRule(requestLimit);

        // Act
        var result = rule.Validate(requestNumber);

        // Assert
        return result;
    }
}