using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class CooldownPeriodTests
    {
        private CooldownPeriod _cooldownPeriod;
        private UserSettings _userSettings;
        private RuleSettings _ruleSettings;
        private ValidationRequest _request;

        [SetUp]
        public void Setup()
        {
            _cooldownPeriod = new CooldownPeriod();
            _userSettings = new UserSettings { Id = "user1" };
            _request = new ValidationRequest { RequestTime = DateTime.UtcNow };

            _ruleSettings = new RuleSettings
            {
                Name = "CooldownPeriod",
                Enabled = true,
                Options = new List<Option>()
                {
                    new Option { Type = "Default", CooldownMsec = 1000 },
                    new Option { Region = "EU", CooldownMsec = 2000 },
                    new Option { Region = "US", CooldownMsec = 500 }
                }
            };
        }

        [Test]
        public void ValidateRequest_RuleNotEnabled_ShouldPassValidation()
        {
            _ruleSettings.Enabled = false;
            var result = _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_NoHistory_ShouldPassValidation()
        {
            var result = _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_WithinCooldown_ShouldFailValidation()
        {
            _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            _request.RequestTime = _request.RequestTime.AddMilliseconds(500);
            var result = _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateRequest_OutsideCooldown_ShouldPassValidation()
        {
            _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            _request.RequestTime = _request.RequestTime.AddMilliseconds(1500);
            var result = _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_RegionOverridesDefault_ShouldUseRegionCooldown()
        {
            _userSettings.Region = Region.EU;

            var result = _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsTrue(result);

            _request.RequestTime = _request.RequestTime.AddMilliseconds(1500);
            result = _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsFalse(result);

            _request.RequestTime = _request.RequestTime.AddMilliseconds(2500);
            result = _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_TierOverridesRegion_ShouldUseTierCooldown()
        {
            _userSettings.ServiceTier = ServiceTier.Pro;

            _ruleSettings = new RuleSettings
            {
                Name = "CooldownPeriod",
                Enabled = true,
                Options = new List<Option>()
                {
                    new Option { Type = "Default", CooldownMsec = 1000 },
                    new Option { Region = "EU", CooldownMsec = 2000 },
                    new Option { Tier = "Pro", CooldownMsec = 500 }
                }
            };

            var result = _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsTrue(result);

            _request.RequestTime = _request.RequestTime.AddMilliseconds(400);
            result = _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsFalse(result);

            _request.RequestTime = _request.RequestTime.AddMilliseconds(600);
            result = _cooldownPeriod.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsTrue(result);
        }
    }

    
}
