namespace RateLimiter
{
	public class ResourceRules
	{
		// // Max requests per timespan Rule
		/// <summary>
		/// Allowed requests count per timespan
		/// </summary>
		public int RequestPerTimespan { get; set; }

		/// <summary>
		/// Moving timespan (in milliseconds)
		/// </summary>
		public int Timespan { get; set; }


		// Min time between requests Rule
		/// <summary>
		/// A certain timespan passed since the last call (in milliseconds)
		/// </summary>
		public int MinCallTimespan { get; set; }
	}
}
