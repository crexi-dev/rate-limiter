using System;

namespace RateLimiter
{
	/// <summary>
	/// Represents a request made to the API.
	/// </summary>
	/// <typeparam name="T">The type of the user's identifier.</typeparam>
	public interface IApiRequest<T>
	{
		/// <summary>
		/// The identifier for the user making the request. This maps
		/// to the user id value in the user's access token.
		/// </summary>
		public T UserId { get; }

		/// <summary>
		/// The timestamp of the request in UTC.
		/// </summary>
		public DateTimeOffset Timestamp { get; }

		/// <summary>
		/// The resource that was accessed by this request.
		/// </summary>
		public string Resource { get; }
	}
}
