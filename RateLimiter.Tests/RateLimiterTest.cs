using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private RateLimitManager _manager;

        [SetUp]
        public void Setup() 
        {
            //Setup various rules for clients
            List<LimitRule> rules = new List<LimitRule>()
            {
                new LimitRule() { Endpoint = "*", MaxRequests = 1, PeriodTimespan = TimeSpan.FromSeconds(2.0) }
            };
            ClientRules cr = new ClientRules("client-1", rules);
            List<ClientRules> cl = new List<ClientRules>();
            cl.Add(cr);

            rules = new List<LimitRule>()
            {
                new LimitRule() { Endpoint = "*", WaitPeriod = true, PeriodTimespan = TimeSpan.FromSeconds(1.0) }
            };
            cr = new ClientRules("client-2", rules);
            cl.Add(cr);

            _manager = new RateLimitManager(cl);
        }

        [TestCase("client-1", 2)]
        [TestCase("client-2", 2)]
        public void Test_MaxRequests(string clientId, int requestCount)
        {
            bool allowed = false;
            for (int i = 0; i < requestCount; i++) 
            {
                allowed = _manager.IsAllowed(clientId);
            }
            Assert.IsTrue(allowed);
        }
    }
}
