using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class RuleAttributeTests
{
    private IServiceProvider _serviceProvider;
    private IRuleContextProvider _contextProvider;

    private const string AllowedClient = "client1";
    private const string NonAllowedClient = "client2";

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _contextProvider = Substitute.For<IRuleContextProvider>();

        _serviceProvider.GetService(typeof(IRuleContextProvider)).Returns(_contextProvider);
    }

    [Test]
    public async Task IsAllowedAsync_ShouldReturnTrue_IfNotAppliable()
    {
        var rule = new TestRuleAttribute();
        _contextProvider.IsAppliableAsync(CancellationToken.None).Returns(true);
        _contextProvider.GetClientIdAsync(CancellationToken.None).Returns(AllowedClient);

        var result = await rule.IsAllowedAsync(_serviceProvider, CancellationToken.None);

        result.Should().BeTrue();
    }

    [TestCase(AllowedClient, true)]
    [TestCase(NonAllowedClient, false)]
    public async Task IsAllowedAsync_ShouldReturnFromInheritor_IfAppliable(string clientId, bool expectedResult)
    {
        var rule = new TestRuleAttribute();
        _contextProvider.IsAppliableAsync(CancellationToken.None).Returns(Task.FromResult(true));
        _contextProvider.GetClientIdAsync(CancellationToken.None).Returns(Task.FromResult(clientId));

        var result = await rule.IsAllowedAsync(_serviceProvider, CancellationToken.None);

        result.Should().Be(expectedResult);
    }

    private class TestRuleAttribute : RuleAttribute<IRuleContextProvider>
    {
        protected override Task<bool> IsAllowedAsync(string? clientId, IServiceProvider sp, CancellationToken ct)
        {
            return Task.FromResult(clientId == AllowedClient);
        }
    }
}