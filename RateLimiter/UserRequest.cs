using System;

namespace RateLimiter
{
    public class UserRequest
    {
        public RequestState State { get; set; }
        public Token Token { get; }
        public DateTime RequestTime { get; set; }

        public UserRequest(Region region, int userId)
        {
            Token = new Token(region, userId);
        }
    }
}
