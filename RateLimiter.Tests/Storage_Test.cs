using NUnit.Framework;
namespace RateLimiter.Tests;
using RateLimiter.Storage;
using System;

[TestFixture]
public class Storage_Test
{
    Storage Store;

    Guid session1;
    Guid session2;
    Guid session3;

    [SetUp]
    public void Init()
    {
        Store = new Storage();
        session1 = Guid.NewGuid();
        session2 = Guid.NewGuid();
        session3 = Guid.NewGuid();
    }


    [Test]
    public void Check_storage_items_consistency()
    {
        Store = new Storage();
         
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
        Store = new Storage();

        // add 100 unique visits
        for (int i = 0; i < 100; i++)
        {
            Store.AddOrAppend(session1);
        }

        Store.Remove("customPasscode", session1);
        Assert.IsTrue(!Store.Exist(session1));
    }

}