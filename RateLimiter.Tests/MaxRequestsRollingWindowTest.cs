using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter;

namespace RateLimiter.Tests
{
    public class MaxRequestsRollingWindowTests
    {
        private MaxRequestsRollingWindow _maxRequestsRollingWindow;
        private UserSettings _userSettings;
        private RuleSettings _ruleSettings;
        private ValidationRequest _request;

        [SetUp]
        public void Setup()
        {
            _maxRequestsRollingWindow = new MaxRequestsRollingWindow();
            _userSettings = new UserSettings { Id = "user1" };
            _request = new ValidationRequest { RequestTime = DateTime.UtcNow };

            _ruleSettings = new RuleSettings
            {
                Name = "MaxRequestsRollingWindow",
                Enabled = true,
                Options = new List<Option>
                {
                    new Option { Type = "Default", MaxRequests = 3, PeriodMsec = 1000 },
                    new Option { Region = "EU", MaxRequests = 5, PeriodMsec = 2000 },
                    new Option { Region = "US", MaxRequests = 10, PeriodMsec = 2000 }
                }
            };
        }

        [Test]
        public void ValidateRequest_WithinLimit_ShouldPassValidation()
        {
            for (int i = 0; i < 3; i++)
            {
                var request = new ValidationRequest { RequestTime = _request.RequestTime.AddMilliseconds(i * 200) };
                var result = _maxRequestsRollingWindow.ValidateRequest(_userSettings, _ruleSettings, request);
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void ValidateRequest_ExceedsLimit_ShouldFailValidation()
        {
            for (int i = 0; i < 3; i++)
            {
                var request = new ValidationRequest { RequestTime = _request.RequestTime.AddMilliseconds(i * 200) };
                _maxRequestsRollingWindow.ValidateRequest(_userSettings, _ruleSettings,request);
            }

            _request.RequestTime = _request.RequestTime.AddMilliseconds(600);
            var result = _maxRequestsRollingWindow.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateRequest_RegionOverridesDefault_ShouldUseRegionSettings()
        {
           _userSettings.Region = Region.EU;

            for (int i = 0; i < 5; i++)
            {
                var request = new ValidationRequest { RequestTime = _request.RequestTime.AddMilliseconds(i * 300) };
                var result = _maxRequestsRollingWindow.ValidateRequest(_userSettings, _ruleSettings, request);
                Assert.IsTrue(result);
            }

            _request.RequestTime = _request.RequestTime.AddMilliseconds(1500);
            var finalResult = _maxRequestsRollingWindow.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsFalse(finalResult);
        }

        [Test]
        public void ValidateRequest_RemoveOldTimestamps_ShouldPassValidation()
        {
            var request = new ValidationRequest { RequestTime =  _request.RequestTime.AddMilliseconds(-1500) };
            _maxRequestsRollingWindow.ValidateRequest(_userSettings, _ruleSettings, request);

            request.RequestTime = _request.RequestTime.AddMilliseconds(-1000);
            _maxRequestsRollingWindow.ValidateRequest(_userSettings, _ruleSettings, request);

            request.RequestTime = _request.RequestTime.AddMilliseconds(-500);
            _maxRequestsRollingWindow.ValidateRequest(_userSettings, _ruleSettings, request);

            var result = _maxRequestsRollingWindow.ValidateRequest(_userSettings, _ruleSettings, _request);
            Assert.IsTrue(result);
        }
    }
}