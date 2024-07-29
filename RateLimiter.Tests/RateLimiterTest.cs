using NUnit.Framework;
using System;
using System.Collections.Generic;
using NSubstitute;

namespace RateLimiter.Tests
{
    public class RequestValidatorTests
    {
        private RequestValidator _requestValidator;
        private UserSettings _userSettings;
        private RuleSettingsList _ruleSettings;
        private ValidationRequest _request;
        private IRequestValidatorRule _ruleMock;

        [SetUp]
        public void Setup()
        {
            _requestValidator = new RequestValidator();
            _userSettings = new UserSettings { Id = "user1" };
            _request = new ValidationRequest {RequestTime = DateTime.UtcNow };
            _ruleMock = Substitute.For<IRequestValidatorRule>();
			_ruleSettings = new RuleSettingsList { 
				RateLimiterRules = new List<RuleSettings>{
					new RuleSettings
                    {
                        Name = _ruleMock.GetType().Name,
                        Enabled = true,
                        Options = new List<Option>()
                    }
				}
			};
        }

		[Test]
        public void CheckRules_NoRulesProvided_ReturnsTrue()
        {
            var rules = new List<IRequestValidatorRule>();

            var result = _requestValidator.CheckRules(_userSettings, _ruleSettings, rules, _request);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckRules_RuleWithNoMatchingSettings_ReturnsFalse()
        {
            var rule = Substitute.For<IRequestValidatorRule>();
            var rules = new List<IRequestValidatorRule> { rule };

            var result = _requestValidator.CheckRules(_userSettings, _ruleSettings, rules, _request);

            Assert.IsFalse(result);
        }

        [Test]
        public void CheckRules_RuleWithMatchingSettingsButNoOptions_ReturnsFalse()
        {
            var rule = Substitute.For<IRequestValidatorRule>();
            var rules = new List<IRequestValidatorRule> { rule };
            var ruleName = rule.GetType().Name;
            _ruleSettings.RateLimiterRules.Add(new RuleSettings { Name = ruleName, Enabled = true, Options = null });

            var result = _requestValidator.CheckRules(_userSettings, _ruleSettings, rules, _request);

            Assert.IsFalse(result);
        }

        [Test]
        public void CheckRules_RuleIsNotEnabled_SkipsRule()
        {
            var rule = Substitute.For<IRequestValidatorRule>();
            var rules = new List<IRequestValidatorRule> { rule };
            var ruleName = rule.GetType().Name;
			_ruleSettings.RateLimiterRules.Clear();
            _ruleSettings.RateLimiterRules.Add(new RuleSettings { Name = ruleName, Enabled = false, Options = new List<Option>() });

            var result = _requestValidator.CheckRules(_userSettings, _ruleSettings, rules, _request);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckRules_AllRulesValid_ShouldReturnTrue()
        {
            _ruleMock.ValidateRequest(_userSettings, Arg.Any<RuleSettings>(), _request).Returns(true);

            var rules = new List<IRequestValidatorRule> { _ruleMock, _ruleMock, _ruleMock };

            var result = _requestValidator.CheckRules(_userSettings, _ruleSettings, rules, _request);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckRules_OneRuleInvalid_ShouldReturnFalse()
        {
            _ruleMock.ValidateRequest(_userSettings, Arg.Any<RuleSettings>(), _request).Returns(true, false, true);

            var rules = new List<IRequestValidatorRule> { _ruleMock, _ruleMock, _ruleMock };

            var result = _requestValidator.CheckRules(_userSettings, _ruleSettings, rules, _request);

            Assert.IsFalse(result);
        }

        [Test]
        public void CheckRules_NoRules_ShouldReturnTrue()
        {
            var rules = new List<IRequestValidatorRule>();

            var result = _requestValidator.CheckRules(_userSettings, _ruleSettings, rules, _request);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckRules_FirstRuleInvalid_ShouldReturnFalse()
        {
            _ruleMock.ValidateRequest(_userSettings, Arg.Any<RuleSettings>(), _request).Returns(false);

            var rules = new List<IRequestValidatorRule> { _ruleMock };

            var result = _requestValidator.CheckRules(_userSettings, _ruleSettings, rules, _request);

            Assert.IsFalse(result);
        }

        [Test]
        public void CheckRules_MixedValidInvalidRules_ShouldReturnFalse()
        {
            var validRuleMock = Substitute.For<IRequestValidatorRule>();
            var invalidRuleMock = Substitute.For<IRequestValidatorRule>();

            validRuleMock.ValidateRequest(_userSettings, Arg.Any<RuleSettings>(), _request).Returns(true);
            invalidRuleMock.ValidateRequest(_userSettings, Arg.Any<RuleSettings>(), _request).Returns(false);

            var rules = new List<IRequestValidatorRule> { validRuleMock, invalidRuleMock, validRuleMock };

            var result = _requestValidator.CheckRules(_userSettings, _ruleSettings, rules, _request);

            Assert.IsFalse(result);
        }
    }
}
