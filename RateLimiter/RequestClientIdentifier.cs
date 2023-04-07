using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RequestClientIdentifier : IClientIdentifier
    {
        // it can be overriden get client from request header or token
        public string Id => "1";

    }
}
