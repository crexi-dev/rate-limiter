using FluentAssertions;
using RateLimit.Contracts;
using RateLimit.DTO;
using RateLimit.Rules;
using Xunit;

namespace RateLimit.Tests.Integration.Rules
{
    public class RuleBTest
	{
		public static IEnumerable<object[]> GetTestRuleBData()
		{
			yield return new object[]
			{
				new RequestDto() { ClientId = "Client1", Region = "EU" }
			};
		}
		[Theory]
		[MemberData(nameof(GetTestRuleBData))]
		public async void Integration_RuleB_Max_1requestPer3Minutes_Test(RequestDto dto)
		{
			var ruleB = ServiceProviderHelper.GetServices<IRule>().OfType<RuleB>().First();
			var result = await ruleB.IsAccessAllowedAsync(dto);
			result.Should().BeTrue();
			result = await ruleB.IsAccessAllowedAsync(dto);
			result.Should().BeFalse();
		}

	}
}
