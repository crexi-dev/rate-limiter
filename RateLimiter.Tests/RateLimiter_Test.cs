using NUnit.Framework;
namespace RateLimiter.Tests;
using RateLimiter.Storage;
using System;
using NSubstitute;
using RateLimiter.Rules;
using RateLimiter.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic;
using NSubstitute.Routing.Handlers;

[TestFixture]
public class RateLimiter_Test
{

    IServiceCollection services = new ServiceCollection();
    ServiceProvider serviceProvider;

    [SetUp]
    public void Init()
    {
        services.AddSingleton<IRateLimiterConfigs, RateLimiterConfigs>();
        services.AddSingleton<IRulesEvaluator, RulesEvaluator>();
        serviceProvider = services.BuildServiceProvider(); 
    }

    [Test]
    public void check_dependency_before_init()
    {
        var configs = Substitute.For<IRateLimiterConfigs>();
        configs.BindConfig().Returns(new Models.ConfigValues()
        {
            Enabled = true,
            MaxAllowed = 20,    // 20 calls max
            TimeFrame = 5       // each 5 seconds
        });

        var Limiter = serviceProvider.GetService<IRulesEvaluator>();
        Limiter = new RulesEvaluator(configs);
        Assert.IsNotNull(Limiter);
        Assert.IsInstanceOfType<IRulesEvaluator>(Limiter);
    }



    [TestCase("8/5/2024 10:25:01 PM", "8/5/2024 10:25:06 PM", 15, true, Description = "15 calls per 5 seconds")]
    [TestCase("8/5/2024 10:25:01 PM", "8/5/2024 10:25:06 PM", 25, false, Description = "25 call per 5 seconds")]
    public void Check_Can_Access_rules(DateTime start, DateTime last, int callsCount, bool shouldPass)
    {
        var configs = Substitute.For<IRateLimiterConfigs>();
        configs.BindConfig().Returns(new Models.ConfigValues()
        {
            Enabled = true,
            MaxAllowed = 20,    // 20 calls max
            TimeFrame = 5       // each 5 seconds
        });

        var limiter = serviceProvider.GetService<IRulesEvaluator>();
        limiter = new RulesEvaluator(configs);
        bool? canAccess = limiter?.CanAccess(start, last, callsCount);

        Assert.IsTrue(shouldPass == canAccess);
    }
}