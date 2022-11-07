using NUnit.Framework;
using RateLimiter.Interface;
using System;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
 
        [Test]
        public void FrequenceLimiterNormal()
        {
            // limit to 3 requests per 5 seconds
            var fl = new FrequenceLimiter(3, TimeSpan.FromSeconds(5));
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));
            Thread.Sleep(500);
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));
            Thread.Sleep(500);
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));
            Thread.Sleep(500);
            Assert.IsFalse(fl.Validate("1"));
            Assert.IsFalse(fl.Validate("2"));
            Thread.Sleep(4100);
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));
        }

        [Test]
        public void IntervalLimiterNormal()
        {
            // limit to 3 requests per 5 seconds
            var il = new IntervalLimiter(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(il.Validate("1"));
            Assert.IsTrue(il.Validate("2"));
            Thread.Sleep(500);
            Assert.IsTrue(il.Validate("1"));
            Assert.IsTrue(il.Validate("2"));
            Thread.Sleep(500);
            Assert.IsTrue(il.Validate("1"));
            Assert.IsTrue(il.Validate("2"));
            Thread.Sleep(300);
            Assert.IsFalse(il.Validate("1"));
            Assert.IsFalse(il.Validate("2"));
            Thread.Sleep(500);
            Assert.IsTrue(il.Validate("1"));
            Assert.IsTrue(il.Validate("2"));
        }

        [Test]
        public void AggreagetdLimiterNormal()
        {
            // limit to 3 requests per 5 seconds
            var fl = new AggreagatedLimiter(new ILimiter[] { new FrequenceLimiter(3, TimeSpan.FromSeconds(5)), new IntervalLimiter(TimeSpan.FromMilliseconds(500)) });
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));
            Thread.Sleep(500);
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));
            Thread.Sleep(500);
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));
            Thread.Sleep(300);
            // cut by interval
            Assert.IsFalse(fl.Validate("1"));
            Assert.IsFalse(fl.Validate("2"));
            Thread.Sleep(5000);
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));
            Thread.Sleep(500);
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));
            Thread.Sleep(500);
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));
            Thread.Sleep(500);
            //cut by frequence
            Assert.IsFalse(fl.Validate("1"));
            Assert.IsFalse(fl.Validate("2"));
            Thread.Sleep(4100);
            Assert.IsTrue(fl.Validate("1"));
            Assert.IsTrue(fl.Validate("2"));


        }
    }
}
