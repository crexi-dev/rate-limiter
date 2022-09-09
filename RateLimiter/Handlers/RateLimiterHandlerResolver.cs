namespace RateLimiter.Handlers
{
    public class RateLimiterHandlerResolver : IRateLimiterHandlerResolver
    {
        public IRateLimiterHandler Resolve(string clientKey)
        {
            var region = clientKey.Split('_')[0];

            if (region == "US")
                return new USRateLimiterHandler();
            else
                return new EURateLimiterHandler();
        } 
    }
}
