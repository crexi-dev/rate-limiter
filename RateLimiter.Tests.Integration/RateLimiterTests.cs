using RateLimiter.Tests.Integration.Setup;

namespace RateLimiter.Tests.Integration
{
    public class RateLimiterTests : BaseClassFixture
    {
        private const string testApiPath = "/test";
        private const string ip = "::1";

        public RateLimiterTests(RateLimiterWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task NoAuthToken_LastCallTimeSpan_Violation()
        {
            // Arrange
            var responseStatusCode = 0;

            // Act
            for (int i = 0; i < 3; i++)
            {
                var request = new HttpRequestMessage(new HttpMethod("GET"), testApiPath);
                request.Headers.Add("X-Real-IP", ip);

                var response = await HttpClient.SendAsync(request);
                responseStatusCode = (int)response.StatusCode;
            }

            // Assert
            Assert.Equal(429, responseStatusCode);
        }
    }
}