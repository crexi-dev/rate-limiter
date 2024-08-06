using NUnit.Framework;
namespace RateLimiter.Tests;

using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RateLimiter.Configs;
using RateLimiter.Storage;
using RateLimiter.Utilities;
using System;

[TestFixture]
public class Storage_Test
{
    Storage Store;

    Guid session1;
    Guid session2;
    Guid session3;

    IServiceCollection services = new ServiceCollection();
    ServiceProvider serviceProvider;

    [SetUp]
    public void Init()
    {
        services.AddSingleton<IRateLimiterConfigs, RateLimiterConfigs>();

        services.AddSingleton<IDateTimeService, DateTimeService>();

        serviceProvider = services.BuildServiceProvider();

        session1 = Guid.NewGuid();
        session2 = Guid.NewGuid();
        session3 = Guid.NewGuid();


        /// mock config service
        var configs = Substitute.For<IRateLimiterConfigs>();
        var time = Substitute.For<IDateTimeService>();

        configs.BindConfig().Returns(new Models.ConfigValues()
        {
            Enabled = true,
            MaxAllowed = 20,    // 20 calls max
            TimeFrame = 5       // each 5 seconds
        });

        time.GetCurrentTime().Returns(DateTime.Now);

        Store = new Storage(configs, time);
    }


    [Test]
    public void Check_storage_items_consistency()
    {
        // add 100 unique visits
        for (int i = 0; i < 100; i++)
        {
            Store.AddOrAppend(session1);
            Store.AddOrAppend(session2);
            Store.AddOrAppend(session3);
        }

        /// each visit got saved successfully
        Assert.IsTrue(Store.Exist(session1));
        Assert.IsTrue(Store.Exist(session2));
        Assert.IsTrue(Store.Exist(session3));

        /// each session has 100 visits logged
        Assert.IsTrue(Store?.Get(session1)?.Count == 100);
        Assert.IsTrue(Store?.Get(session2)?.Count == 100);
        Assert.IsTrue(Store?.Get(session3)?.Count == 100);
    }

    [Test]
    public void Check_storage_items_Remove()
    {
        // add 100 unique visits
        for (int i = 0; i < 100; i++)
        {
            Store.AddOrAppend(session1);
        }

        Store.Remove("customPasscode", session1);
        Assert.IsTrue(!Store.Exist(session1));
    }
}