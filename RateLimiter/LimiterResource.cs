using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Rules;

namespace RateLimiter
{
    public class LimiterResource
    {
        public string Name { get; private set; }

        public List<IRule> Rules { get; private set; }

        public LimiterResource(string name, List<IRule> rules)
        {
            Name = name;
            Rules = rules;
        }
    }
}
