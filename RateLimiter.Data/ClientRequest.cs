using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.DataModel
{
    public class ClientRequest
    {
        public int ClientId { get; set; }
        public int ResourceId { get; set; }
    }
}
