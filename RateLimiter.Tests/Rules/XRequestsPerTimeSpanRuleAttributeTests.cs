using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class XRequestsPerTimeSpanRuleAttributeTests
{
    private IServiceProvider _serviceProvider;
    private IDateTimeProvider _dateTimeProvider;
    private IStore _store;
    private IRuleContextProvider _contextProvider;
    private ILogger<XRequestsPerTimeSpanRuleAttribute<IRuleContextProvider>> _logger;
    
    private const string ClientId = "client_id";
    private const string Key = $"RateLimiting:XRequestsPerTimeSpanRule:{ClientId}";

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _store = Substitute.For<IStore>();
        _contextProvider = Substitute.For<IRuleContextProvider>();
        _logger = Substitute.For<ILogger<XRequestsPerTimeSpanRuleAttribute<IRuleContextProvider>>>();

        _serviceProvider.GetService(typeof(IDateTimeProvider)).Returns(_dateTimeProvider);
        _serviceProvider.GetService(typeof(IStore)).Returns(_store);
        _serviceProvider.GetService(typeof(IRuleContextProvider)).Returns(_contextProvider);
        _serviceProvider.GetService(typeof(ILogger<XRequestsPerTimeSpanRuleAttribute<IRuleContextProvider>>)).Returns(_logger);

        _contextProvider.IsAppliableAsync(CancellationToken.None).Returns(true);
        _contextProvider.GetClientIdAsync(CancellationToken.None).Returns(ClientId);
    }

    [Test]
    public async Task ShouldAllowRequest_WhenStoreIsEmpty()
    {
        var rule = new XRequestsPerTimeSpanRuleAttribute<IRuleContextProvider>(5, "00:30:00");
        var now = DateTime.UtcNow;
        _dateTimeProvider.UtcNow.Returns(now);
        _store.GetAsync<XRequestsPerTimeSpanRuleEntry?>(Key, CancellationToken.None)
            .Returns((XRequestsPerTimeSpanRuleEntry?)null);

        var result = await rule.IsAllowedAsync(_serviceProvider, CancellationToken.None);

        result.Should().BeTrue();
        await _store.Received(1).SetAsync(Key, new XRequestsPerTimeSpanRuleEntry(1, now), CancellationToken.None);
    }

    [Test]
    public async Task ShouldAllowRequest_WhenCountIsBelowMax()
    {
        var rule = new XRequestsPerTimeSpanRuleAttribute<IRuleContextProvider>(5, "00:30:00");
        var now = DateTime.UtcNow;
        _dateTimeProvider.UtcNow.Returns(now);
        _store.GetAsync<XRequestsPerTimeSpanRuleEntry?>(Key, CancellationToken.None).Returns(new XRequestsPerTimeSpanRuleEntry(4, now));

        var result = await rule.IsAllowedAsync(_serviceProvider, CancellationToken.None);

        result.Should().BeTrue();
        await _store.Received(1).SetAsync(Key, new XRequestsPerTimeSpanRuleEntry(5, now), CancellationToken.None);
    }

    [Test]
    public async Task ShouldDenyRequest_WhenCountExceedsMax()
    {
        var rule = new XRequestsPerTimeSpanRuleAttribute<IRuleContextProvider>(4, "00:30:00");
        var now = DateTime.UtcNow;
        _dateTimeProvider.UtcNow.Returns(now);
        _store.GetAsync<XRequestsPerTimeSpanRuleEntry?>(Key, CancellationToken.None).Returns(new XRequestsPerTimeSpanRuleEntry(5, now));

        var result = await rule.IsAllowedAsync(_serviceProvider, CancellationToken.None);

        result.Should().BeFalse();
        await _store.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<XRequestsPerTimeSpanRuleEntry>(), CancellationToken.None);
    }

    [Test]
    public async Task ShouldResetCount_WhenTimeSpanHasElapsed()
    {
        var rule = new XRequestsPerTimeSpanRuleAttribute<IRuleContextProvider>(5, "00:30:00");
        var pastTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));
        var now = DateTime.UtcNow;
        _dateTimeProvider.UtcNow.Returns(now);
        _store.GetAsync<XRequestsPerTimeSpanRuleEntry?>(Key, CancellationToken.None).Returns(new XRequestsPerTimeSpanRuleEntry(5, pastTime));

        var result = await rule.IsAllowedAsync(_serviceProvider, CancellationToken.None);

        result.Should().BeTrue();
        await _store.Received(1).SetAsync(Key, new XRequestsPerTimeSpanRuleEntry(1, now), CancellationToken.None);
    }
}