using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class ConfigValues
    {
        public bool? Enabled { get; set; }
        public int? MaxAllowed { get; set; }
        public int? TimeFrame { get; set; }
    }
}
