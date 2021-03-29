using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.Contexts
{
    public class ClientContext
    {
        public string Token { get; set; }

        public string GeoLocation { get; set; }
    }
}
