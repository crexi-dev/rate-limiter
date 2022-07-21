using RateLimiter.Model;
using System.Net;

namespace RateLimiter.LocationService
{
    public interface ILocationService 
    {
        Regions GetRegionFromIp(IPAddress iPAddress);
    }
}
