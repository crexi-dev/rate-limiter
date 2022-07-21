using RateLimiter.Model;
using System.Net;

namespace RateLimiter.LocationService
{
    public class LocationService : ILocationService
    {
        //Use one of these location provider to determine request Regions
        //https://ipdata.co/
        //https://ipinfo.io/
        
        public Regions GetRegionFromIp(IPAddress iPAddress)
        {
            return Regions.US;
        }
    }
}
