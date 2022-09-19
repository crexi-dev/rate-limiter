namespace RateLimiter.Rules
{
    /// <summary>
    /// A sample for request structure
    /// </summary>
    public class Request
    {
        public int Size { get; set; }   

        public int ProcessDurationMs { get; set; }
    }
}
