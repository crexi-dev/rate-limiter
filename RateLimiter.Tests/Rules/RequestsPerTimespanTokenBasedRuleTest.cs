using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class RequestsPerTimespanTokenBasedRuleTest
{
    [Test]
    public async Task OneThreadCheckTest()
    {
        using var usRule = new RequestsPerTimespanTokenBasedRule(TimeSpan.FromSeconds(5), 2, "^US.*");
        using var euRule = new RequestsPerTimespanTokenBasedRule(TimeSpan.FromSeconds(5), 2, "^EU.*");
        using (CancellationTokenSource source = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
        {
            (await usRule.CheckAsync(new("US Token 1"), source.Token)).Should().BeTrue();
            (await euRule.CheckAsync(new("US Token 1"), source.Token)).Should().BeTrue();
            (await usRule.CheckAsync(new("EU Token 1"), source.Token)).Should().BeTrue();
            (await euRule.CheckAsync(new("EU Token 1"), source.Token)).Should().BeTrue();
            (await usRule.CheckAsync(new("US Token 2"), source.Token)).Should().BeTrue();
            (await euRule.CheckAsync(new("US Token 2"), source.Token)).Should().BeTrue();
            (await usRule.CheckAsync(new("EU Token 2"), source.Token)).Should().BeTrue();
            (await euRule.CheckAsync(new("EU Token 2"), source.Token)).Should().BeTrue();
            await FluentActions.Awaiting(async () => await usRule.CheckAsync(new("US Token 3"), source.Token))
                .Should().ThrowAsync<OperationCanceledException>();
            (await euRule.CheckAsync(new("US Token 3"), source.Token)).Should().BeTrue();
            (await usRule.CheckAsync(new("EU Token 3"), source.Token)).Should().BeTrue();   
            await FluentActions.Awaiting(async () => await euRule.CheckAsync(new("EU Token 3"), source.Token))
                .Should().ThrowAsync<OperationCanceledException>();
        }
    }
}