using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Classes
{
    public class ClientRequest : IClientRequest
    {
        private string _token;
        private GeoLocation _geo_location;
        private DateTime _callDateTimeStamp;
        private string _ipAddress;

        public string Token { get => _token; set => _token = value; }
        public GeoLocation GeoLocation { get => _geo_location; set => _geo_location = value; }
        public DateTime CallDateTimeStamp { get => _callDateTimeStamp; set => _callDateTimeStamp = value; }

        public ClientRequest(string token, string ipAddress)
        {
            _token = token;
            _ipAddress = ipAddress;
            _callDateTimeStamp = DateTime.Now;
            _geo_location =
                ipAddress.StartsWith("172") ? GeoLocation.US : GeoLocation.EU; // mockup
        }
    }

    public enum GeoLocation
    {
        US,
        EU
    }
}
