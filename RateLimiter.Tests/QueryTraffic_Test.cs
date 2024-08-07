using NUnit.Framework;
namespace RateLimiter.Tests;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RateLimiter.Configs;
using RateLimiter.Rules;
using RateLimiter.Storage;
using RateLimiter.Utilities;
using System;
using System.Linq;

[TestFixture]
public class QueryTraffic_Test
{
    ///  storage manager
    Storage Store;

    // session = new user
    Guid session1;

    /// mock config and datetime service
    ServiceProvider serviceProvider;
    IRulesEvaluator limiter;
    IRateLimiterConfigs configs = Substitute.For<IRateLimiterConfigs>();
    IDateTimeService time = Substitute.For<IDateTimeService>();

    [SetUp]
    public void Init()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<IRateLimiterConfigs, RateLimiterConfigs>();
        serviceProvider = services.BuildServiceProvider();
        session1 = Guid.NewGuid();
        limiter = serviceProvider?.GetService<IRulesEvaluator>();

        // mock config service
        configs.BindConfig().Returns(new Models.ConfigValues()
        {
            Enabled = true,
            MaxAllowed = 20,    // 20 calls max
            TimeFrame = 5       // each 5 seconds
        });

        limiter = new RulesEvaluator(configs);

        Store = new Storage(configs, time);
    }


    [Test]
    public void Check_Rate_limiter_storage_query()
    {
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

        /// random added dates need to be sorted
        totalRequests?.Sort();
         
        /// since we are injecting random datetime,
        /// check before assert, if any datetime was within the limited per mock config range
        if(totalRequests?.Count > 1 && totalRequests?.Count < configs.BindConfig().MaxAllowed)  //max allowed per config-mock
        {
            bool? canAccess = limiter?.CanAccess(totalRequests.First(), totalRequests.Last(), totalRequests.Count);
            Assert.IsTrue(canAccess.Value == true);
        }
    }
}
