using System;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Services;

namespace RateLimiter.Tests;

public class WindowLimitRateValidatorTests
{
    private WindowLimitRateValidator _validator;
    [SetUp]
    public void Init()
    {
        _validator = new WindowLimitRateValidator(TimeSpan.FromSeconds(5), 5);
    }

    [Test]
    public void LastVisitLessTimespanAndVisitCountsLessLimit_Validate_ShouldReturnTrue()
    {
        //Arrange
        var lastVisit = DateTime.UtcNow.AddSeconds(-10);

        //Act
        var result = _validator.Validate(new ClientData { LastVisit = lastVisit, VisitCounts = 4});

        //Assert
        result.Result.Should().BeTrue();
    }

    [Test]
    public void LastVisitGreaterTimespanAndVisitCountsLessLimit_Validate_ShouldReturnTrue()
    {
        //Arrange
        var lastVisit = DateTime.UtcNow.AddSeconds(-3);

        //Act
        var result = _validator.Validate(new ClientData { LastVisit = lastVisit, VisitCounts = 4});

        //Assert
        result.Result.Should().BeTrue();
        result.VisitCounts.Should().Be(5);
    }

    [Test]
    public void LastVisitGreaterTimespanAndVisitCountsEqualsLimit_Validate_ShouldReturnTrue()
    {
        //Arrange
        var lastVisit = DateTime.UtcNow.AddSeconds(-3);

        //Act
        var result = _validator.Validate(new ClientData { LastVisit = lastVisit, VisitCounts = 5});

        //Assert
        result.Result.Should().BeFalse();
        result.VisitCounts.Should().Be(5);
    }
}