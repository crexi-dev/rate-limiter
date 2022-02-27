namespace RateLimiter
{
	public interface IAccessToken<T>
	{
		public T UserId { get; set; }
		public string IPAddress { get; }
		public string CountryCode { get; }
	}
}
