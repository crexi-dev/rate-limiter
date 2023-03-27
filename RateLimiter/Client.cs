using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class Client
    {
        public Client(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}
