using Moq;
using NUnit.Framework;
using RateLimiter.Repositories;
using RateLimiter.Repositories.Detectors;
using RateLimiter.Rules;
using RateLimiter.Rules.Constructors;
using RateLimiter.RuleTemplates;
using Shouldly;
using System;

namespace RateLimiter.Tests.Repositories.Detectors;

[TestFixture]
public class RuleConstructorDetectorTests
{
    

    private RuleConstructorDetector CreateRuleConstructorDetector()
    {
        return new RuleConstructorDetector();
    }

    [Test]
    public void GetConstructor_RuleConstructorType_ReturnsValidRuleConstructor()
    {
        // Arrange
        var ruleConstructorDetector = this.CreateRuleConstructorDetector();
        Type constructorType = typeof(RuleConstructor);

        // Act
        var result = ruleConstructorDetector.GetConstructor(constructorType);

        // Assert
        result
            .ShouldNotBeNull()
            .ShouldBeAssignableTo<IRuleConstructor>();
    }

    [Test]
    public void GetConstructor_NotRuleConstructorType_InvalidConstructorTypeException()
    {
        // Arrange
        var ruleConstructorDetector = this.CreateRuleConstructorDetector();
        Type constructorType = typeof(NotRuleConstructor);

        // Act
        var result = () => { ruleConstructorDetector.GetConstructor(constructorType); };

        // Assert
        result.ShouldThrow<InvalidRuleConstructorType>();
    }

    private class NotRuleConstructor
    {

    }

    private class RuleConstructor : IRuleConstructor
    {
        public IRule Construct(RuleTemplateParams templateParams)
        {
            throw new NotImplementedException();
        }
    }
}
