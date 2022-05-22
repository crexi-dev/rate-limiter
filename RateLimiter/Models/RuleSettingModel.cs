using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public  class RuleSettingModel
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public long Period { get; set; }
        public long Limit { get; set; }
    }
}
