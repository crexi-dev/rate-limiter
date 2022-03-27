using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Contracts
{
    public interface ITimeService
    {
        public DateTime Now { get; }
        public long TimeStamp { get; }

    }
}
