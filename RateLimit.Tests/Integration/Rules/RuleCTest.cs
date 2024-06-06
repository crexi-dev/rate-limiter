using FluentAssertions;
using RateLimit.Contracts;
using RateLimit.DTO;
using RateLimit.Rules;
using Xunit;

namespace RateLimit.Tests.Integration.Rules
{
    public class RuleCTest
	{
		public static IEnumerable<object[]> GetTestRuleCData()
		{
			yield return new object[]
			{
				new RequestDto() { ClientId = "Client1", Region = "NotDefined" }, false, false, false
			};
			yield return new object[]
			{
				new RequestDto() { ClientId = "Client1", Region = "US" }, true, true, false
			};
			yield return new object[]
			{
				new RequestDto() { ClientId = "Client2", Region = "eu" }, true, false, false
			};
		}

		[Theory]
		[MemberData(nameof(GetTestRuleCData))]
		public async void Integration_RuleC_Test(RequestDto dto, bool fstCall, bool scndCall, bool thrdCall)
		{
			var ruleC = ServiceProviderHelper.GetServices<IRule>().OfType<RuleC>().First();
			var result = await ruleC.IsAccessAllowedAsync(dto);
			result.Should().Be(fstCall);
			result = await ruleC.IsAccessAllowedAsync(dto);
			result.Should().Be(scndCall);
			result = await ruleC.IsAccessAllowedAsync(dto);
			result.Should().Be(thrdCall);
		}
	}
}
