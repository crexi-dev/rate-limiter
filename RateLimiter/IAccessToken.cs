namespace RateLimiter
{
	/// <summary>
	/// The token that is included with all requests to the API.
	/// </summary>
	/// <typeparam name="T">The type of the user's identifier.</typeparam>
	public interface IAccessToken<T>
	{
		public T UserId { get; }
		public string IPAddress { get; }
		public string CountryCode { get; }
	}
}
