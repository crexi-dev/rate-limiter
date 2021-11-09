namespace RateLimiter.Application.AccessRestriction.Authorization
{
    /// <inheritdoc />
    public class UserToken : IUserToken
    {
        /// <inheritdoc />
        public int UserId { get; set; }
    }
}