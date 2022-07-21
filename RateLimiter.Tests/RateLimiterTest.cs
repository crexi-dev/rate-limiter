using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RateLimiter.Cache;
using RateLimiter.Model;
using RateLimiter.RulesEngine;
using RateLimiter.RulesEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTests
    {
        private IRuleEngine engine;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.UseRateLimiter();
            services.AddSingleton<IStorageService, InMemoryService>();
            services.AddSingleton(typeof(IConfiguration), Configuration.GetConfiguration);
            var serviceProvider = services.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();
            engine = scope.ServiceProvider.GetRequiredService<IRuleEngine>();
        }

        [TestCase(Regions.EU)]
        public void TestEURuleInWindow_Successful(Regions region)
        {

            //Arrange 
            var ip = IPAddress.Parse("127.0.0.1");
            var token = new Token(ip);
            var request1 = new UserRequest(token, region, DateTime.UtcNow);
            var request2 = new UserRequest(token, region, DateTime.UtcNow.AddMilliseconds(100));

            //Act
            var validate1 = engine.ProcessRules(request1);
            var validate2 = engine.ProcessRules(request2);

            //Assert
            Assert.IsTrue(validate1);
            Assert.IsTrue(validate2);
        }

        [TestCase(Regions.EU)]
        public void TestEURuleOutOfWindow_Successful(Regions region)
        {

            //Arrange 
            var ip = IPAddress.Parse("127.0.0.1");
            var token = new Token(ip);
            var request1 = new UserRequest(token, region, DateTime.UtcNow);
            var request2 = new UserRequest(token, region, DateTime.UtcNow.AddMilliseconds(5));

            //Act
            var validate1 = engine.ProcessRules(request1);
            var validate2 = engine.ProcessRules(request2);

            //Assert
            Assert.IsTrue(validate1);
            Assert.IsFalse(validate2);
        }

        [TestCase(Regions.US)]
        [TestCase(Regions.EU)]
        [TestCase(Regions.Others)]
        public void TestUSOtherRuleInWindow_Successful(Regions region)
        {

            //Arrange 
            var ip = IPAddress.Parse("127.0.0.1");
            var token = new Token(ip);
            var request1 = new UserRequest(token, region, DateTime.UtcNow);
            var request2 = new UserRequest(token, region, DateTime.UtcNow.AddMilliseconds(100));

            //Act
            var validate1 = engine.ProcessRules(request1);
            var validate2 = engine.ProcessRules(request2);

            //Assert
            Assert.IsTrue(validate1);
            Assert.IsTrue(validate2);
        }

        [TestCase(Regions.US)]
        [TestCase(Regions.Others)]
        public void TestUSOtherRuleOutOfWindow_Successful(Regions region)
        {

            //Arrange 
            var ip = IPAddress.Parse("127.0.0.1");
            var token = new Token(ip);
            var request1 = new UserRequest(token, region, DateTime.UtcNow);
            var request2 = new UserRequest(token, region, DateTime.UtcNow.AddMilliseconds(5));

            //Act
            var validate1 = engine.ProcessRules(request1);
            var validate2 = engine.ProcessRules(request2);

            //Assert
            Assert.IsTrue(validate1);
            Assert.IsTrue(validate2);
        }

        [TestCase(Regions.US)]
        [TestCase(Regions.Others)]
        
        public void TestUSMaxRequestInWindows_Successfully(Regions region)
        {
            //Arrage
            var ip = IPAddress.Parse("127.0.0.1");
            var token = new Token(ip);
            var request = new UserRequest(token, region, DateTime.UtcNow);
            var results = new List<bool>();

            //Act
            for (int i = 0; i < 10; i++)
            {
                results.Add(engine.ProcessRules(request));
            }


            //Assert
            Assert.IsTrue(results.All(x => x));
        }

        [TestCase(Regions.US)]
        public void TestUSMaxRequestInWindows_ExceedingMax_Successfully(Regions region)
        {
            //Arrage
            var ip = IPAddress.Parse("127.0.0.1");
            var token = new Token(ip);
            var request = new UserRequest(token, region, DateTime.UtcNow);

            //Act
            for (int i = 0; i < 10; i++)
            {
                engine.ProcessRules(request);
            }

            var result = engine.ProcessRules(request);

            //Assert
            Assert.IsFalse(result);
        }

        [TestCase(Regions.Others)]
        public void OtherRegionsPassAllRules_Successfully(Regions region)
        {
            //Arrange
            var ip = IPAddress.Parse("127.0.0.1");
            var token = new Token(ip);
            var request = new UserRequest(token, region, DateTime.UtcNow);
            bool result = true;

            //Act
            for (int i = 0; i < 100; i++)
            {
                request = new UserRequest(token, region, DateTime.UtcNow.AddMilliseconds(i));
                result = result && engine.ProcessRules(request);
            }

            //Assert
            Assert.IsTrue(result);
        }
    }
}