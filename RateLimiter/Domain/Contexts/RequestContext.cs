using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.Contexts
{
    public class RequestContext
    {
        public string Resource { get; set; }

        public ClientContext ClientContext { get; set; }

        public long TimeStamp { get; set; }
    }
}
