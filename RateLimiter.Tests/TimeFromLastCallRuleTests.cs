using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class TimeFromLastCallRuleTests
    {
        [Test]
        public void Success()
        {
            var rule = new TimeFromLastCallRule
            {
                TimeSpan = TimeSpan.FromSeconds(1)
            };

            var now = DateTime.UtcNow;

            Assert.That(rule.Check(new[] { now, now.AddSeconds(2) }), Is.True);
        }

        [Test]
        public void Fail()
        {
            var rule = new TimeFromLastCallRule
            {
                TimeSpan = TimeSpan.FromSeconds(2)
            };

            var now = DateTime.UtcNow;

            Assert.That(rule.Check(new[] { now, now.AddSeconds(1) }), Is.False);
        }
    }
}
