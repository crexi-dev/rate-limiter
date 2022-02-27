namespace RateLimiter.Tests
{
	internal class AccessToken : IAccessToken<int>
	{
		public int UserId { get; init; }

		public string IPAddress { get; init; } = string.Empty;

		public string CountryCode { get; init; } = string.Empty;
	}
}
