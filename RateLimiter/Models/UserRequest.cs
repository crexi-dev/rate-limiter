using System;

namespace RateLimiter.Model
{
    public class UserRequest
    {
        public Token Token { get; }
        public Regions Region { get; }
        public DateTime RequestTime { get; }

        public UserRequest(Token token, Regions region) : this(token, region, DateTime.UtcNow) { }
        public UserRequest(Token token, Regions region, DateTime requestTime)
        {
            Token = token;
            Region = region;
            RequestTime = requestTime;
        }
    }
}
