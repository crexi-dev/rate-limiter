using System;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Services;

namespace RateLimiter.Tests;

public class TimespanRateValidatorTests
{
    private TimespanRateValidator _validator;
    [SetUp]
    public void Init()
    {
        _validator = new TimespanRateValidator(TimeSpan.FromSeconds(5));
    }

    [Test]
    public void LastVisitLessTimespan_Validate_ShouldReturnTrue()
    {
        //Arrange
        var lastVisit = DateTime.UtcNow.AddSeconds(-10);

        //Act
        var result = _validator.Validate(new ClientData { LastVisit = lastVisit });

        //Assert
        result.Result.Should().BeTrue();
    }

    [Test]
    public void LastVisitGreaterTimespan_Validate_ShouldReturnFalse()
    {
        //Arrange
        var lastVisit = DateTime.UtcNow.AddSeconds(-3);

        //Act
        var result = _validator.Validate(new ClientData { LastVisit = lastVisit });

        //Assert
        result.Result.Should().BeFalse();
    }
}