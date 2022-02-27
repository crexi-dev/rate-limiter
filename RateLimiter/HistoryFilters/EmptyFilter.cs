namespace RateLimiter.RequestFilters
{
	/// <summary>
	/// A default request filter that doesn't filter out any requests.
	/// </summary>
	public class EmptyFilter<T> : IRequestFilter<T>
	{
		public bool IsRequestIncluded(IApiRequest<T> request) => true;
	}
}
