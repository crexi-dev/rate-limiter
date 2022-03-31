using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Application;
using RateLimiter.Application.Interfaces;
using RateLimiter.Rules;
using RateLimiter.Rules.Extensions;
using RateLimiter.Tests.Extensions;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class ResourceRulesConfigurationTests
    {
        private readonly Dictionary<string, AccessRule> _resourceRules;

        public ResourceRulesConfigurationTests()
        {
            var cache = new ApplicationCache("ApplicationCache");
            _resourceRules = ConfigureResourceRules(cache);
        }

        [Test]
        public void SendAllowedCountRequestsForUSUsersResource_TestShouldPass()
        {
            var userJwt = "TestUserJwtUS"; // should contains US
            var resource = "users";
            var rule = _resourceRules[resource];

            // no more then 50 request per 2 seconds
            for (var i = 0; i < 50; i++)
            {
                rule.Validate(resource, userJwt).Should().BeTrue();
            }
        }

        [Test]
        public void SendAllowedIntervalRequestsForEUUsersResource_TestShouldPass()
        {
            var userJwt = "TestUserJwtEU"; // should contains EU
            var resource = "users";
            var rule = _resourceRules[resource];

            // interval between request should be >= 100 
            for (var i = 0; i < 20; i++)
            {
                Thread.Sleep(100);
                rule.Validate(resource, userJwt).Should().BeTrue();
            }
        }

        [Test]
        public void SendAllowedCountWithAllowedIntervalRequestsForItemsResource_TestShouldPass()
        {
            var userJwt = "TestUserJwt";
            var resource = "items";
            var rule = _resourceRules[resource];

            // no more then 50 request per 2 seconds with the at least 30 milliseconds interval
            for (var i = 0; i < 50; i++)
            {
                Thread.Sleep(30);
                rule.Validate(resource, userJwt).Should().BeTrue();
            }
        }

        private Dictionary<string, AccessRule> ConfigureResourceRules(ICache cache)
        {
            var usersResource = "users";
            var itemsResource = "items";

            var usUsersAccessRule = new PeriodAccessRule(cache, TimeSpanExtensions.FromSeconds(2), 50, usersResource);
            var euUsersAccessRule = new IntervalAccessRule(cache, TimeSpanExtensions.FromMilliseconds(100), usersResource);

            var usersAccessRule = new PredicateAccessRule(new (Func<string, string, bool>, AccessRule)[]
            {
                ((resource, key) => key.Contains("US"), usUsersAccessRule),
                ((resource, key) => key.Contains("EU"), euUsersAccessRule),
            });

            var itemsAccessRule = new PeriodAccessRule(cache, TimeSpanExtensions.FromSeconds(2), 50, itemsResource)
                .And(new IntervalAccessRule(cache, TimeSpanExtensions.FromMilliseconds(30), itemsResource));

            return new Dictionary<string, AccessRule>
            {
                {usersResource, usersAccessRule},
                {itemsResource, itemsAccessRule},
            };
        }
    }
}
