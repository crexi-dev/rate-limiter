namespace RateLimiter
{
    /// <summary>
    /// Is used to specify an endoint on which <see cref="RateLimiterMiddleware">RateLimiterMiddleware</see> should be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RateLimiterAttribute : Attribute
    {
        /// <summary>
        /// A certain timespan passed since the last call in milliseconds. 1000ms is a default.
        /// </summary>
        public int Timespan { get; set; } = 1000;

        /// <summary>
        /// X requests per timespan is available. 1 is a default.
        /// </summary>
        public int MaxRequests { get; set; } = 1;
    }
}