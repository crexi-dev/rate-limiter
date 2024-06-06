using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RateLimit.Contracts;
using RateLimit.DTO;
using RateLimit.Options;
using RateLimit.Restictions;
using Xunit;

namespace RateLimit.Tests.Restrictions
{
	public class XRequestPerDatespanRestrictionsTest
	{
		[Theory]
		[InlineData("client_id", 3, 1)]
		public async void RuleA_CorrectRestrictionAssigned_Test(string clientId, int limit, int fromMinuteTimespan)
		{
			//Arrange
			var options = new Options.RuleOptions() { Limit = limit, Period = TimeSpan.FromMinutes(fromMinuteTimespan) };
			var mockOptions = new Mock<IOptions<RuleOptions>>();
			mockOptions.Setup(ap => ap.Value).Returns(options);
			var mockService = new Mock<ILimitService>();
			mockService.Setup(x => x.IsAccessAllowedAsync(limit, TimeSpan.FromMinutes(fromMinuteTimespan), clientId)).Returns(Task.FromResult(true));

			var restriction = new XRequestPerDatespanRestictions(mockOptions.Object, mockService.Object);


			//Act
			var result = await restriction.IsPassedAsync(new RequestDto { ClientId = clientId, Region = "eu" });

			//Assert
			mockService.Verify(x => x.IsAccessAllowedAsync(limit, TimeSpan.FromMinutes(fromMinuteTimespan), clientId), Times.Once());
			result.Should().BeTrue();
		}
	}
}
