using NUnit.Framework;
namespace RateLimiter.Tests;

using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RateLimiter.Configs;
using RateLimiter.Rules;
using RateLimiter.Storage;
using RateLimiter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class QueryTraffic_Test
{
    Storage Store;

    Guid session1;

    IServiceCollection services = new ServiceCollection();
    ServiceProvider serviceProvider;

    [SetUp]
    public void Init()
    {
        services.AddSingleton<IRateLimiterConfigs, RateLimiterConfigs>();
        serviceProvider = services.BuildServiceProvider();
        session1 = Guid.NewGuid(); 
    }


    [Test]
    public void Check_Rate_limiter_storage_query()
    {
        /// mock config service
        var configs = Substitute.For<IRateLimiterConfigs>();
        var time = Substitute.For<IDateTimeService>();

        configs.BindConfig().Returns(new Models.ConfigValues()
        {
            Enabled = true,
            MaxAllowed = 20,    // 20 calls max
            TimeFrame = 5       // each 5 seconds
        });

        var limiter = serviceProvider.GetService<IRulesEvaluator>();
        limiter = new RulesEvaluator(configs);

        Store = new Storage(configs, time);

        Random r = new Random();

        // add 100 visits per session into past 1/2 hr 
        for (int i = 0; i < 100; i++)
        {
            int randomHalfHour = r.Next(1, 30);
            randomHalfHour *= -1;
            /// add past 10 minutes time frame
            time.GetCurrentTime().Returns(DateTime.Now.AddMinutes(randomHalfHour));
            Store.AddOrAppend(session1);
        }

        Assert.NotNull(Store?.Get(session1));

        var totalRequests = Store?.Get(session1);

        totalRequests?.Sort();
         
        /// since we are injecting random datetime,
        /// check before assert, if any datetime was within the limited per mock config range
        if(totalRequests?.Count > 1)
        {
            bool? canAccess = limiter?.CanAccess(totalRequests.First(), totalRequests.Last(), totalRequests.Count);
            Assert.IsTrue(canAccess.Value == true);
        }
    }
}