using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Core.Models
{
    public class RateLimitterCounter
    {
        public string Token { get; set; }
        public string Resource { get; set; }
        public DateTime Date { get; set; }
    }
}
