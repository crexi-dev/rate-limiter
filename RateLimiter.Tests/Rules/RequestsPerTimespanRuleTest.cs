using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class RequestsPerTimespanRuleTest
{
    [Test]
    public async Task OneThreadCheckTest()
    {
        using var rule = new RequestsPerTimespanRule(TimeSpan.FromSeconds(2), 2);

        using (CancellationTokenSource source = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
        {
            (await rule.CheckAsync(new("Token 1"), source.Token)).Should().BeTrue();
            (await rule.CheckAsync(new("Token 2"), source.Token)).Should().BeTrue();
            await FluentActions.Awaiting(async () => await rule.CheckAsync(new("Token 3"), source.Token))
                .Should().ThrowAsync<OperationCanceledException>();
        }
        
        Thread.Sleep(TimeSpan.FromSeconds(2));
        
        using (CancellationTokenSource source = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
        {
            (await rule.CheckAsync(new("Token 1"), source.Token)).Should().BeTrue();
            (await rule.CheckAsync(new("Token 2"), source.Token)).Should().BeTrue();
            (await rule.CheckAsync(new("Token 3"), source.Token)).Should().BeTrue();
        }
    }
    
    [Test]
    public void MultiThreadsCheckTest()
    {
        using(var rule = new RequestsPerTimespanRule(TimeSpan.FromSeconds(3), 2))
        {
            var t = new Thread(() =>
            {
                using var source = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                rule.CheckAsync(new("Token 1"), source.Token).GetAwaiter().GetResult()
                    .Should().BeTrue();
            })
            {
                Name = "Thread_1"
            };
            t.Start();
            t = new Thread(() =>
            {
                using var source = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                rule.CheckAsync(new("Token 1"), source.Token).GetAwaiter().GetResult()
                    .Should().BeTrue();
            })
            {
                Name = "Thread_2"
            };
            t.Start();

            t = new Thread(() =>
            {
                using var source = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                FluentActions.Invoking(() => rule.CheckAsync(new("Token 3"), source.Token).GetAwaiter().GetResult())
                    .Should().Throw<OperationCanceledException>();
            })
            {
                Name = "Thread_3"
            };
            t.Start();

            t = new Thread(() =>
            {
                using var source = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                rule.CheckAsync(new("Token 4"), source.Token).GetAwaiter().GetResult()
                    .Should().BeTrue();
            })
            {
                Name = "Thread_4"
            };
            t.Start();
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }
    }
}