namespace RateLimiter.RequestFilters
{
	/// <summary>
	/// Filters a user's history to only requests that apply to the associated rule.
	/// </summary>
	public interface IRequestFilter<T>
	{
		/// <summary>
		/// Determines whether a request should be included in the inspected history.
		/// </summary>
		/// <returns>
		/// True if the request should be included when checking rate limits, false otherwise.
		/// </returns>
		bool IsRequestIncluded(IApiRequest<T> request);
	}
}
