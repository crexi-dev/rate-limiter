namespace RateLimiter.Extensions
{
    public static class UserTokeExtensions
    {
        public static string GetClientType(this string token) => token.ToLower(); //Here we can replace any logic how to retrieve real client type
    }
}
