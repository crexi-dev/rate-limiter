using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RequestModel
    {
        public RequestModel(DateTime date)
        {
            RequestDate = date;
        }
        public DateTime RequestDate { get; private set; }
    }
}
