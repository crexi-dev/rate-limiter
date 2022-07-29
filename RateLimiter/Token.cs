namespace RateLimiter
{
    public class Token
    {
        public Token(Region region, int userId)
        {
            Region = region;
            UserId = userId;
        }

        public Region Region { get; }
        public int UserId { get; }
    }
}
