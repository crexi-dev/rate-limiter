using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Services;
using RateLimiter.Tests.Helpers;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimitingServiceTest
{
    private static readonly object[] TestCases =
    {
        new object[] { null, "US-token" },
        new object[] { "", "US-token" },
        new object[] { "  ", "US-token" },
        new object[] { "resource", null },
        new object[] { "resource", "" },
        new object[] { "resource", "  " },
    };
    private IRuleProvider _ruleProvider;
    private ILogger<RateLimitingService> _logger;

    [SetUp]
    public void SetUp()
    {
        _ruleProvider = new Mock<IRuleProvider>().Object;
        _logger = new Mock<ILogger<RateLimitingService>>().Object;
    }    

    [Test, TestCaseSource(nameof(TestCases))]
    public void IsRequestAllowedAsync_ThrowsArgumentException_WhenInputInvalid(string resource, string token)
    {
        var rateLimitingService = new RateLimitingService(_ruleProvider, _logger);

        Assert.ThrowsAsync<ArgumentException>(() => rateLimitingService.ValidateRequestAsync(resource, token));
    }
}