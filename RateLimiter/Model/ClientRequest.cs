using System;

namespace RateLimiter.Model
{
    public class ClientRequest
    {
        public ClientToken ClientToken { get; }
        public ClientLocations ClientLocation { get; }
        public DateTime RequestTime { get; }

        public ClientRequest(ClientToken clientToken, ClientLocations clientLocation, DateTime requestTime)
        {
            ClientToken = clientToken;
            ClientLocation = clientLocation;
            RequestTime = requestTime;
        }
    }
}