using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RateLimiter.DataAccess;
using RateLimiter.Model;
using RateLimiter.RulesEngine.Interfaces;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.RulesEngine;
using RateLimiter.Extensions;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private IPAddress fakeIp = IPAddress.Parse("192.168.1.1");
        private IRuleEngine rulesEngine;

        [SetUp]
        public void Setup()
        {
            var requiredServices = new ServiceCollection();

            requiredServices.AddRateLimitServices();
            requiredServices.AddSingleton<IStorage, InMemoryCache>();
            requiredServices.AddSingleton(typeof(IConfiguration), new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    { "requestLimit", "10" },
                    { "requestTimeSpan", "1000" },
                    { "betweenRequestsTimeSpan", "10" }
                }).Build());
            var serviceProvider = requiredServices.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();
            rulesEngine = scope.ServiceProvider.GetRequiredService<IRuleEngine>();
        }

        [TestCase]
        public void Should_Return_False_Exceed_Max_Requests_US()
        {
            var clientToken = new ClientToken(fakeIp);
            var clientRequest = new ClientRequest(clientToken, ClientLocations.US, DateTime.UtcNow);

            //Run this for max requests of 10
            for (int i = 0; i < 10; i++)
            {
                rulesEngine.ProcessRules(clientRequest);
            }

            var isRulePassed = rulesEngine.ProcessRules(clientRequest);

            Assert.IsFalse(isRulePassed);
        }

        [TestCase]
        public void Should_Return_True_WithIn_Request_Limit_US()
        {
            var clientToken = new ClientToken(fakeIp);
            var clientRequest = new ClientRequest(clientToken, ClientLocations.US, DateTime.UtcNow);

            var allRulesPassed = true;
            //Run this for max requests of 10
            for (int i = 0; i < 5; i++)
            {
                if (!rulesEngine.ProcessRules(clientRequest))
                    allRulesPassed = false;
            }

            Assert.IsTrue(allRulesPassed);
        }

        [TestCase]
        public void Should_Return_False_Exceed_Timespan__EU()
        {
            var token = new ClientToken(fakeIp);
            var clientRequest_Current = new ClientRequest(token, ClientLocations.EU, DateTime.UtcNow);
            var clientRequest_OutOfWindow = new ClientRequest(token, ClientLocations.EU, DateTime.UtcNow.AddMilliseconds(2));

            Assert.IsTrue(rulesEngine.ProcessRules(clientRequest_Current));
            Assert.IsFalse(rulesEngine.ProcessRules(clientRequest_OutOfWindow));
        }

        [TestCase]
        public void Should_Return_True_Within_Timespan__EU()
        {
            var token = new ClientToken(fakeIp);
            var clientRequest_Current = new ClientRequest(token, ClientLocations.EU, DateTime.UtcNow);
            var clientRequest_InWindow = new ClientRequest(token, ClientLocations.EU, DateTime.UtcNow.AddMilliseconds(100));

            Assert.IsTrue(rulesEngine.ProcessRules(clientRequest_Current));
            Assert.IsTrue(rulesEngine.ProcessRules(clientRequest_InWindow));
        }
    }
}
