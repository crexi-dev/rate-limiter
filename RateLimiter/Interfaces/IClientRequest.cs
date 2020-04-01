using RateLimiter.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces
{
    public interface IClientRequest
    {
        string Token { get; set; }
        GeoLocation GeoLocation { get; set; }
        DateTime CallDateTimeStamp { get; set; }
    }
}
