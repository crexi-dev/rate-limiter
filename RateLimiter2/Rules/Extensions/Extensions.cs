using System.Net;

namespace RateLimiter.Rules.Extensions
{
    public static class Extensions
    {
        // TokenInRegion() extension method is for demonstraton only. A real-world system would
        // probably extract the JWT from the request header and decode it to detrmine the region.
        public static bool TokenInRegion(this string request, string region)
        {
            if (!string.IsNullOrEmpty(request))
                return request.StartsWith(region);

            return false;
        }
    }
}
