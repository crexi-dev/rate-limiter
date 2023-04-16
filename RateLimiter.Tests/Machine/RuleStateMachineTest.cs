using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RateLimiter.Machine;
using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Machine;

[TestFixture]
public class RuleStateMachineTest
{
    [Test]
    public async Task StepTest()
    {
        var machine = (RuleStateMachine<RequestData>)RuleStateMachineBuilder.Build(new[]
        {
            new AlwaysTrueRule<RequestData>()
        }, NullLogger.Instance);
        var actual = await machine.StepAsync(new("token"), State.InitState,
            CancellationToken.None);
        actual.Should().Be(new State(false, 2));
        actual = await machine.StepAsync(new("token"), actual, CancellationToken.None);
        actual.Should().Be(new State(true, 3));
    }
    
    [Test]
    public async Task RunTest()
    {
        var machine = RuleStateMachineBuilder.Build(new[]
        {
            new AlwaysTrueRule<RequestData>()
        }, NullLogger.Instance);
        bool actual = await machine.RunAsync(new("token"), CancellationToken.None);
        actual.Should().Be(true);
    }
}