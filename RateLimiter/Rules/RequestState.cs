namespace RateLimiter.Rules
{
    /// <summary>
    /// The current processing state of the request by the rules engine
    /// </summary>
    public enum RequestState
    {
        Unhandled = 0,
        Accepted,
        Denied,
    }
}
