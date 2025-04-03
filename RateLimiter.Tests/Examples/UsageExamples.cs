using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RateLimiter.Abstractions;
using RateLimiter.Extensions;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Examples
{
    [TestFixture]
    public class UsageExamples
    {
        [Test]
        public void Example_BasicConfiguration()
        {
            var services = new ServiceCollection();
            
            services.AddRateLimiter(options =>
            {
                var requestCountRule = new RequestCountRule(new Storage.InMemoryRequestTracker())
                {
                    MaxRequests = 100,
                    TimeWindow = TimeSpan.FromMinutes(1)
                };
                
                options.DefaultRules.Add(requestCountRule);
            });
            
            var serviceProvider = services.BuildServiceProvider();
            
            Assert.Pass("Example code compiles successfully");
        }
        
        [Test]
        public void Example_AdvancedConfiguration()
        {
            var services = new ServiceCollection();
            
            services.AddRateLimiter(options =>
            {
                var requestTracker = new Storage.InMemoryRequestTracker();
                
                var requestCountRule = new RequestCountRule(requestTracker)
                {
                    MaxRequests = 100,
                    TimeWindow = TimeSpan.FromMinutes(1)
                };
                
                var timeSinceLastCallRule = new TimeSinceLastCallRule(requestTracker)
                {
                    MinInterval = TimeSpan.FromSeconds(1)
                };
                
                var regionRule = new RegionCompositeRule(requestCountRule);
                
                var euRule = new TimeSinceLastCallRule(requestTracker)
                {
                    MinInterval = TimeSpan.FromSeconds(2)
                };
                regionRule.AddRegionRule("EU", euRule);
                
                options.DefaultRules.Add(regionRule);
                
                options.EndpointRules.Add("/api/high-load", new List<IRateLimitRule>
                {
                    new RequestCountRule(requestTracker)
                    {
                        MaxRequests = 10,
                        TimeWindow = TimeSpan.FromMinutes(1)
                    }
                });
            });
            
            var serviceProvider = services.BuildServiceProvider();
            
            Assert.Pass("Example code compiles successfully");
        }
    }
}