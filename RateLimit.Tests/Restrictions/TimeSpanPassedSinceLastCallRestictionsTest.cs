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
	public class TimeSpanPassedSinceLastCallRestictionsTest
	{
		[Theory]
		[InlineData("client_id", 990)]
		public async void RuleA_CorrectRestrictionAssigned_Test(string clientId, int lastCallPeriod)
		{
			//Arrange
			var options = new Options.RuleOptions() { Limit = It.IsAny<int>(), Period = It.IsAny<TimeSpan>(), LastCallPeriod = TimeSpan.FromMinutes(lastCallPeriod) };
			var mockOptions = new Mock<IOptions<RuleOptions>>();
			mockOptions.Setup(ap => ap.Value).Returns(options);
			var mockService = new Mock<ILimitService>();

			mockService.Setup(x => x.IsAccessAllowedAsync(1, TimeSpan.FromMinutes(lastCallPeriod), clientId)).Returns(Task.FromResult(true));

			var restriction = new TimeSpanPassedSinceLastCallRestictions(mockOptions.Object, mockService.Object);

			//Act
			var result = await restriction.IsPassedAsync(new RequestDto { ClientId = clientId, Region = "eu" });

			//Assert
			mockService.Verify(x => x.IsAccessAllowedAsync(1, TimeSpan.FromMinutes(lastCallPeriod), clientId), Times.Once());
			result.Should().BeTrue();
		}

	}
}
