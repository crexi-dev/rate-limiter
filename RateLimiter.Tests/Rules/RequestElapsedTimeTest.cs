using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.State;
using System;
using System.Threading;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    internal class RequestElapsedTimeTest
    {
        [OneTimeSetUp]
        public void StartUp()
        {
            _ = InMemoryRuleState.GetInstance;
        }

        [Test]
        public void First_Request()
        {
            var access_token = Guid.NewGuid().ToString();
            var mem = InMemoryRuleState.GetInstance;

            var stored_tm = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");
            Assert.IsFalse(stored_tm.Item2);

            var rule = new RequestElapsedTime(mem, TimeSpan.FromSeconds(10));

            var rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);
            var store_time = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");

            Assert.IsTrue(store_time.Item2);
        }

        [Test]
        public void Second_Request_InsideElapsed()
        {
            var access_token = Guid.NewGuid().ToString();
            var mem = InMemoryRuleState.GetInstance;

            var rule = new RequestElapsedTime(mem, TimeSpan.FromSeconds(10));

            var rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            var store_time = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");

            Assert.IsTrue(store_time.Item2);

            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });
            Assert.IsTrue(rslt);
        }

        [Test]
        public void Request_Post_Elapsed()
        {
            var access_token = Guid.NewGuid().ToString();
            var mem = InMemoryRuleState.GetInstance;

            var rule = new RequestElapsedTime(mem, TimeSpan.FromSeconds(1));

            var rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);
            var store_time = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");

            Assert.IsTrue(store_time.Item2);

            Thread.Sleep(2000);

            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });
            Assert.IsFalse(rslt);

            var newtime = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");

            var diff = newtime.Item1.Subtract(store_time.Item1);
            Assert.IsTrue(diff.TotalMilliseconds > 0);
        }
    }
}