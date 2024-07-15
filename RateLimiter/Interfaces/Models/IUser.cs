namespace RateLimiter.Interfaces.Models
{
    /// <summary>
    /// Stores information about the user associated with an HTTP request
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// An id unique to the user associated with the request
        /// </summary>
        string Id { get; }

        // TODO: Add other relevant properties that a rate-limit rule may want to key off of
    }
}
