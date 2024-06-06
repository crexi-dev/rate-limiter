using FluentAssertions;
using FluentAssertions.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimit.Contracts;
using RateLimit.Rules;
using System.Reflection;
using Xunit;

namespace RateLimit.Tests
{
	public class RegistrationTests
	{
		[Fact]
		public void AddRateRule_CheckIRule_NotRegistered()
		{
			var services = new ServiceCollection();
			var sp = services.BuildServiceProvider();

			Assert.Null(sp.GetService<IRule>());
		}

		[Fact]
		public void AddRateRule_CheckIsOnlyRuleARegistered()
		{
			var services = new ServiceCollection();
			var config = new ConfigurationBuilder().AddJsonFile("TestConfig.json", optional: true, reloadOnChange: true).Build();
			services.AddRateRuleAService(config);
			var sp = services.BuildServiceProvider();

			var activeRules = sp.GetService<IRule>();

			activeRules.Should().BeOfType<RuleA>();
		}

		[Fact]
		public void AddRateRule_CheckAllRulesRegistered()
		{
			var services = new ServiceCollection();
			var config = new ConfigurationBuilder().AddJsonFile("TestConfig.json", optional: true, reloadOnChange: true).Build();

			services.AddRateRuleAService(config);
			services.AddRateRuleBService(config);
			services.AddRateRuleCService(config);

			var sp = services.BuildServiceProvider();
			var activeRules = sp.GetServices<IRule>();

			AllTypes.From(Assembly.GetAssembly(typeof(IRule))).ThatImplement<IRule>()
				.Except(activeRules.Select(x => x.GetType()))
				.Should().BeEmpty();
		}
	}
}
