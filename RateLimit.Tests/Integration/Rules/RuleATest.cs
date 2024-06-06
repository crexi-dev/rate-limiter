using FluentAssertions;
using RateLimit.Contracts;
using RateLimit.DTO;
using RateLimit.Rules;
using Xunit;

namespace RateLimit.Tests.Integration.Rules
{
    public class RuleATest
	{
		public static IEnumerable<object[]> GetTestRuleAData()
		{
			yield return new object[]
			{
				new RequestDto() { ClientId = "Client1", Region = "EU" }
			};

		}
		[Theory]
		[MemberData(nameof(GetTestRuleAData))]
		public async void Integration_TextRuleA_Max_2requestPer2Minutes_Test(RequestDto dto)
		{
			var ruleA = ServiceProviderHelper.GetServices<IRule>().OfType<RuleA>().First();
			var result = await ruleA.IsAccessAllowedAsync(dto);
			result.Should().BeTrue();
			result = await ruleA.IsAccessAllowedAsync(dto);
			result.Should().BeTrue();
			result = await ruleA.IsAccessAllowedAsync(dto);
			result.Should().BeFalse();
		}
	}
}
