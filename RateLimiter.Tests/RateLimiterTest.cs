using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using RateLimiter.Models.Enum;
using RateLimiter.Resource;
using RateLimiter.Rule;
using RateLimiter.Rule.Contracts;
using RateLimiter.Storage.Contracts;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private Controller _controller;
        private readonly Mock<IRuleStorage> _ruleStorage = new Mock<IRuleStorage>();
        private readonly Mock<IResourceManager> _resourceManager1Mock = new Mock<IResourceManager>();
        private readonly Mock<IResourceManager> _resourceManager2Mock = new Mock<IResourceManager>();
        private readonly Mock<IResourceManager> _resourceManager3Mock = new Mock<IResourceManager>();
        
        [SetUp]
        public void SetUp()
        {
            _controller = new Controller(_resourceManager1Mock.Object, _resourceManager2Mock.Object,
                _resourceManager3Mock.Object, _ruleStorage.Object);
        }
        
        [Test]
        public void LastCallTest()
        {
            
            string key1 = "key1";
            _ruleStorage.Setup(x => x.GetRule(ResourceType.Res1)).Returns(new List<IRule>
            {
                new LastCallRule { LastCall = TimeSpan.FromMinutes(1) }
            });
            Assert.AreEqual(ResponceType.Success, _controller.GetResource1Data(key1).ResponceType);
            Assert.AreEqual(ResponceType.Fail, _controller.GetResource1Data(key1).ResponceType);
        }

        [Test]
        public void RequestsPerTimespanTest()
        {
            string key1 = "key1";
            _ruleStorage.Setup(x => x.GetRule(ResourceType.Res1)).Returns(new List<IRule>
                { new RequestsPerTimespanRule { RequestsLimit = 1, TimeSpan = TimeSpan.FromSeconds(2)} });
            Assert.AreEqual(ResponceType.Success, _controller.GetResource1Data(key1).ResponceType);
            Assert.AreEqual(ResponceType.Fail, _controller.GetResource1Data(key1).ResponceType);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Assert.AreEqual(ResponceType.Success, _controller.GetResource1Data(key1).ResponceType);
        }

        [Test]
        public void PrefixRuleTest()
        {
            string key1 = "US-key1";
            string key2 = "EU-key1";
            _ruleStorage.Setup(x => x.GetRule(ResourceType.Res1)).Returns(new List<IRule>
            {
                new PrefixReqiestPerTimespanRule { RequestsLimit = 1, TimeSpan = TimeSpan.FromSeconds(2), Prefix = "EU" },
                new PrefixReqiestPerTimespanRule { RequestsLimit = 1, TimeSpan = TimeSpan.FromSeconds(2), Prefix = "US"}
            });
            Assert.AreEqual(ResponceType.Success, _controller.GetResource1Data(key1).ResponceType);
            Assert.AreEqual(ResponceType.Success, _controller.GetResource1Data(key2).ResponceType);
            Assert.AreEqual(ResponceType.Fail, _controller.GetResource1Data(key1).ResponceType);
            Assert.AreEqual(ResponceType.Fail, _controller.GetResource1Data(key2).ResponceType);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Assert.AreEqual(ResponceType.Success, _controller.GetResource1Data(key1).ResponceType);
            Assert.AreEqual(ResponceType.Success, _controller.GetResource1Data(key2).ResponceType);
        }

        [Test]
        public void MixRulesTest()
        {
            string key1 = "US-mixkey1";
            _ruleStorage.Setup(x => x.GetRule(ResourceType.Res1)).Returns(new List<IRule>
            {
                new PrefixReqiestPerTimespanRule { RequestsLimit = 1, TimeSpan = TimeSpan.FromMinutes(1), Prefix = "US"},
                new LastCallRule { LastCall = TimeSpan.FromSeconds(2) }
            });
            Assert.AreEqual(ResponceType.Success, _controller.GetResource1Data(key1).ResponceType);
            Assert.AreEqual(ResponceType.Fail, _controller.GetResource1Data(key1).ResponceType);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Assert.AreEqual(ResponceType.Fail, _controller.GetResource1Data(key1).ResponceType);
        }

        [Test]
        public void DifferentResourceTest()
        {
            string key1 = "US-difreskey";
            _ruleStorage.Setup(x => x.GetRule(ResourceType.Res1)).Returns(new List<IRule>
            {
                new LastCallRule { LastCall = TimeSpan.FromSeconds(2) }
            });
            _ruleStorage.Setup(x => x.GetRule(ResourceType.Res2)).Returns(new List<IRule>
            {
                new LastCallRule { LastCall = TimeSpan.FromSeconds(2) }
            });
            Assert.AreEqual(ResponceType.Success, _controller.GetResource1Data(key1).ResponceType);
            Assert.AreEqual(ResponceType.Fail, _controller.GetResource1Data(key1).ResponceType);
            Assert.AreEqual(ResponceType.Fail, _controller.GetResource2Data(key1).ResponceType);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Assert.AreEqual(ResponceType.Success, _controller.GetResource2Data(key1).ResponceType);
        }
    }
}
