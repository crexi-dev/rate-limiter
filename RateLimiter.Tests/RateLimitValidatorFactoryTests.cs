using System;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Services;

namespace RateLimiter.Tests;

public class RateLimitValidatorFactoryTests
{
    private RateLimitValidatorFactory _factory;

    [SetUp]
    public void Init()
    {
        var settingsMock = new Mock<IOptions<RateLimitSettings>>();
        settingsMock.Setup(x => x.Value).Returns(new RateLimitSettings
        {
            CommonRule = new RateLimitRule(),
            Regions = new[]
            {
                new RateLimitRule
                {
                    RegionKey = "EU",
                    Type = RateLimitRuleType.Timespan,
                    WindowSizeInSeconds = 5
                },
                new RateLimitRule
                {
                    RegionKey = "EU-failed",
                    Type = RateLimitRuleType.Timespan,
                    WindowSizeInSeconds = null
                },
                new RateLimitRule
                {
                    RegionKey = "US",
                    Type = RateLimitRuleType.RequestsPerTimespan,
                    WindowSizeInSeconds = 5,
                    PermitLimit = 5
                },
                new RateLimitRule
                {
                    RegionKey = "US-failed",
                    Type = RateLimitRuleType.RequestsPerTimespan,
                    WindowSizeInSeconds = null,
                    PermitLimit = null
                },
            }
        });
        _factory = new RateLimitValidatorFactory(settingsMock.Object);
    }

    [Test]
    public void EUCorrect_Create_ShouldReturnTimespanValidator()
    {
        //Act
        var validator = _factory.Create("EU");

        //Assert
        validator.Should().BeOfType<TimespanRateValidator>();
    }

    [Test]
    public void EUFailed_Create_ShouldThrowArgumentNullException()
    {
        //Act
        var action = () => _factory.Create("EU-failed");

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void USCorrect_Create_ShouldReturnWindowLimitValidator()
    {
        //Act
        var validator = _factory.Create("US");

        //Assert
        validator.Should().BeOfType<WindowLimitRateValidator>();
    }

    [Test]
    public void USFailed_Create_ShouldThrowArgumentNullException()
    {
        //Act
        var action = () => _factory.Create("US-failed");

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }
}