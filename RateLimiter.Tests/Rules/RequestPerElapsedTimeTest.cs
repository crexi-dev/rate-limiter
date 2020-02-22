using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.State;
using System;
using System.Threading;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    internal class RequestPerElapsedTimeTest
    {
        [OneTimeSetUp]
        public void startup()
        {
            _ = InMemoryRuleState.GetInstance;
        }

        [Test]
        public void First_Request()
        {
            var access_token = Guid.NewGuid().ToString();
            var mem = InMemoryRuleState.GetInstance;

            var rule = new RequestPerElapsedTime(mem, TimeSpan.FromSeconds(10), 3);

            var rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);
            var store_time = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");

            var cnt = mem.Retrieve<int>($"{access_token}{Constants.AppendKeyReqCnt}");

            Assert.IsTrue(store_time.Item2);
            Assert.IsTrue(cnt.Item2);
            Assert.AreEqual(1, cnt.Item1);
        }

        [Test]
        public void Second_Request_Inside_Elapsed()
        {
            var access_token = Guid.NewGuid().ToString();
            var mem = InMemoryRuleState.GetInstance;

            var rule = new RequestPerElapsedTime(mem, TimeSpan.FromSeconds(10), 3);

            var rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            var store_time = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");
            var cnt = mem.Retrieve<int>($"{access_token}{Constants.AppendKeyReqCnt}");

            Assert.IsTrue(store_time.Item2);
            Assert.IsTrue(cnt.Item2);
            Assert.AreEqual(2, cnt.Item1);
        }

        [Test]
        public void Third_Request_Inside_Elapsed()
        {
            var access_token = Guid.NewGuid().ToString();
            var mem = InMemoryRuleState.GetInstance;

            var rule = new RequestPerElapsedTime(mem, TimeSpan.FromSeconds(10), 3);

            var rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            var store_time = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");
            var cnt = mem.Retrieve<int>($"{access_token}{Constants.AppendKeyReqCnt}");

            Assert.IsTrue(store_time.Item2);
            Assert.IsTrue(cnt.Item2);
            Assert.AreEqual(3, cnt.Item1);
        }

        [Test]
        public void Fourth_Request_Inside_Elapsed_GreaterThan_Cnt()
        {
            var access_token = Guid.NewGuid().ToString();
            var mem = InMemoryRuleState.GetInstance;

            var rule = new RequestPerElapsedTime(mem, TimeSpan.FromSeconds(10), 3);

            var rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsTrue(rslt);

            var store_time = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");
            var cnt = mem.Retrieve<int>($"{access_token}{Constants.AppendKeyReqCnt}");

            Assert.IsTrue(store_time.Item2);
            Assert.IsTrue(cnt.Item2);
            Assert.AreEqual(4, cnt.Item1);
        }

        [Test]
        public void Time_Has_Elapsed_Reset()
        {
            var access_token = Guid.NewGuid().ToString();
            var mem = InMemoryRuleState.GetInstance;

            var rule = new RequestPerElapsedTime(mem, TimeSpan.FromSeconds(10), 3);

            var rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            Thread.Sleep(10000);

            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            var store_time = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");
            var cnt = mem.Retrieve<int>($"{access_token}{Constants.AppendKeyReqCnt}");

            Assert.IsTrue(store_time.Item2);
            Assert.IsTrue(cnt.Item2);
            Assert.AreEqual(1, cnt.Item1);
        }

        [Test]
        public void Rule_State_For_Each_Instantiation_Rule()
        {
            var access_token = Guid.NewGuid().ToString();
            var mem = InMemoryRuleState.GetInstance;

            var rule = new RequestPerElapsedTime(mem, TimeSpan.FromSeconds(10), 3);

            var rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            //new instance
            rule = new RequestPerElapsedTime(mem, TimeSpan.FromSeconds(10), 3);
            rslt = rule.Execute(new RequestInfo { Access_Token = access_token });

            Assert.IsFalse(rslt);

            var store_time = mem.Retrieve<DateTime>($"{access_token}{Constants.AppendKeyPrevReqTime}");
            var cnt = mem.Retrieve<int>($"{access_token}{Constants.AppendKeyReqCnt}");

            Assert.IsTrue(store_time.Item2);
            Assert.IsTrue(cnt.Item2);
            Assert.AreEqual(2, cnt.Item1);
        }
    }
}