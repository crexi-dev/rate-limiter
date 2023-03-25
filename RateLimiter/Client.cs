using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class Client
    {
        public Client(ILimiterBucket bucket)
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; }
    }
}
