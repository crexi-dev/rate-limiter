using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RuleDefinition
    {
        public string Name { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }
}
