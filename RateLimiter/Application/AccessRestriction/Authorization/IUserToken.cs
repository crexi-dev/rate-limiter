namespace RateLimiter.Application.AccessRestriction.Authorization
{
    /// <summary>
    /// Represents a user identity.
    /// </summary>
    public interface IUserToken
    {
        /// <summary>
        /// Identifier for a user of this service (could be a person, machine to machine client, etc.)
        /// </summary>
        public int UserId { get; set; }
    }
}
