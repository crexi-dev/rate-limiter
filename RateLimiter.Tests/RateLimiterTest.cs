using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RateLimiter.Services;
using RateLimiter.Tests.Helpers;
using RateLimiterRules.Rules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    private const string ResourceName = "resource";
    private const string UsRegion = "US";
    private const string UsToken = "US-token";
    private const string EuRegion = "EU";
    private const string EuToken = "EU-token";

    private ILogger<RateLimitingService> _logger;
    private MockDateTimeWrapper _mockDateTimeWrapper;

    [SetUp]
    public void SetUp()
    {
        _logger = new Mock<ILogger<RateLimitingService>>().Object;
        _mockDateTimeWrapper = new MockDateTimeWrapper();
    }

    #region Basic Tests

    [TestCase(UsRegion, UsToken, ExpectedResult = true)]
    [TestCase(EuRegion, EuToken, ExpectedResult = true)]
    public async Task<bool> IsRequestAllowedAsync_WithinLimit_ReturnsTrue(string region, string token)
    {
        var ruleService = new RuleProvider(_mockDateTimeWrapper)
            .AddRule(ResourceName, UsRegion, new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), _mockDateTimeWrapper))
            .AddRule(ResourceName, EuRegion, new TimeSinceLastCallRule(TimeSpan.FromSeconds(2), _mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService, _logger);

        return (await rateLimitingService.ValidateRequestAsync(ResourceName, token)).IsAllowed;
    }

    #endregion

    #region TimeSinceLastCallRule Tests

    [TestCase(UsRegion, UsToken, "TimeSinceLastCallExceeded")]
    [TestCase(EuRegion, EuToken, "TimeSinceLastCallExceeded")]
    public async Task IsRequestAllowedAsync_TimeSinceLastCallRule_ExceedsLimit_ReturnsFalseWithErrorCode(string region, string token, string expectedErrorCode)
    {
        var ruleService = new RuleProvider(_mockDateTimeWrapper)
            .AddRule(ResourceName, region, new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), _mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService, _logger);

        await rateLimitingService.ValidateRequestAsync(ResourceName, token);
        _mockDateTimeWrapper.UtcNow = _mockDateTimeWrapper.UtcNow.AddMilliseconds(999);
        var result = await rateLimitingService.ValidateRequestAsync(ResourceName, token);

        Assert.IsFalse(result.IsAllowed);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorCode == expectedErrorCode));
    }

    [TestCase(UsRegion, UsToken)]
    [TestCase(EuRegion, EuToken)]
    public async Task IsRequestAllowedAsync_TimeSinceLastCallRule_WithinLimit_ReturnsTrue(string region, string token)
    {
        var ruleService = new RuleProvider(_mockDateTimeWrapper)
            .AddRule(ResourceName, region, new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), _mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService, _logger);

        await rateLimitingService.ValidateRequestAsync(ResourceName, token);
        _mockDateTimeWrapper.UtcNow = _mockDateTimeWrapper.UtcNow.AddSeconds(2);
        var result = await rateLimitingService.ValidateRequestAsync(ResourceName, token);

        Assert.IsTrue(result.IsAllowed);
        Assert.IsEmpty(result.Errors);
    }

    #endregion

    #region XRequestsPerTimespanRule Tests

    [TestCase(UsRegion, UsToken, "XRequestsPerTimespanExceeded")]
    [TestCase(EuRegion, EuToken, "XRequestsPerTimespanExceeded")]
    public async Task IsRequestAllowedAsync_XRequestsPerTimespanRule_ExceedsLimit_ReturnsFalseWithErrorCode(string region, string token, string expectedErrorCode)
    {
        var ruleService = new RuleProvider(_mockDateTimeWrapper)
            .AddRule(ResourceName, region, new XRequestsPerTimespanRule(2, TimeSpan.FromSeconds(5), _mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService, _logger);

        await rateLimitingService.ValidateRequestAsync(ResourceName, token);
        await rateLimitingService.ValidateRequestAsync(ResourceName, token);
        var result = await rateLimitingService.ValidateRequestAsync(ResourceName, token);

        Assert.IsFalse(result.IsAllowed);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorCode == expectedErrorCode));
    }

    [TestCase(UsRegion, UsToken)]
    [TestCase(EuRegion, EuToken)]
    public async Task IsRequestAllowedAsync_XRequestsPerTimespanRule_WithinLimit_ReturnsTrue(string region, string token)
    {
        var ruleService = new RuleProvider(_mockDateTimeWrapper)
            .AddRule(ResourceName, region, new XRequestsPerTimespanRule(2, TimeSpan.FromSeconds(5), _mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService, _logger);

        await rateLimitingService.ValidateRequestAsync(ResourceName, token);
        var result = await rateLimitingService.ValidateRequestAsync(ResourceName, token);

        Assert.IsTrue(result.IsAllowed);
        Assert.IsEmpty(result.Errors);
    }

    #endregion

    #region Multiple Rules Tests

    [TestCase(UsRegion, UsToken)]
    [TestCase(EuRegion, EuToken)]
    public async Task IsRequestAllowedAsync_MultipleRules_AllPassed_ReturnsTrue(string region, string token)
    {
        var ruleService = new RuleProvider(_mockDateTimeWrapper)
            .AddRule(ResourceName, region, new XRequestsPerTimespanRule(2, TimeSpan.FromSeconds(5), _mockDateTimeWrapper))
            .AddRule(ResourceName, region, new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), _mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService, _logger);

        await rateLimitingService.ValidateRequestAsync(ResourceName, token);
        _mockDateTimeWrapper.UtcNow = _mockDateTimeWrapper.UtcNow.AddSeconds(2);
        var result = await rateLimitingService.ValidateRequestAsync(ResourceName, token);

        Assert.IsTrue(result.IsAllowed);
        Assert.IsEmpty(result.Errors);
    }

    [TestCase(UsRegion, UsToken, "TimeSinceLastCallExceeded")]
    [TestCase(EuRegion, EuToken, "TimeSinceLastCallExceeded")]
    public async Task IsRequestAllowedAsync_MultipleRules_OneError_ReturnsFalseWithErrorCode(string region, string token, string expectedErrorCode)
    {
        var ruleService = new RuleProvider(_mockDateTimeWrapper)
            .AddRule(ResourceName, region, new XRequestsPerTimespanRule(2, TimeSpan.FromSeconds(5), _mockDateTimeWrapper))
            .AddRule(ResourceName, region, new TimeSinceLastCallRule(TimeSpan.FromMilliseconds(500), _mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService, _logger);

        await rateLimitingService.ValidateRequestAsync(ResourceName, token);
        await rateLimitingService.ValidateRequestAsync(ResourceName, token);
        _mockDateTimeWrapper.UtcNow = _mockDateTimeWrapper.UtcNow.AddMilliseconds(499);
        var result = await rateLimitingService.ValidateRequestAsync(ResourceName, token);

        Assert.IsFalse(result.IsAllowed);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorCode == expectedErrorCode));
    }

    [TestCase(UsRegion, UsToken, "TimeSinceLastCallExceeded", "XRequestsPerTimespanExceeded")]
    [TestCase(EuRegion, EuToken, "TimeSinceLastCallExceeded", "XRequestsPerTimespanExceeded")]
    public async Task IsRequestAllowedAsync_MultipleRules_MultipleErrors_ReturnsFalseWithAllErrorCodes(string region, string token, string expectedErrorCode1, string expectedErrorCode2)
    {
        var ruleService = new RuleProvider(_mockDateTimeWrapper)
            .AddRule(ResourceName, region, new XRequestsPerTimespanRule(2, TimeSpan.FromSeconds(5), _mockDateTimeWrapper))
            .AddRule(ResourceName, region, new TimeSinceLastCallRule(TimeSpan.FromMilliseconds(500), _mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService, _logger);

        await rateLimitingService.ValidateRequestAsync(ResourceName, token);
        await rateLimitingService.ValidateRequestAsync(ResourceName, token);
        _mockDateTimeWrapper.UtcNow = _mockDateTimeWrapper.UtcNow.AddMilliseconds(499);
        var result = await rateLimitingService.ValidateRequestAsync(ResourceName, token);

        Assert.IsFalse(result.IsAllowed);
        Assert.AreEqual(2, result.Errors.Length);

        Assert.IsTrue(result.Errors.Any(e => e.ErrorCode == expectedErrorCode1));
        Assert.IsTrue(result.Errors.Any(e => e.ErrorCode == expectedErrorCode2));
    }

    #endregion
}