using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Repository;
using RuleEngine;

namespace RateLimiter.Resources
{
    public class Resource1 : ResourceBase
    {

        public Resource1(string token, RuleEngine.IRulesEngine rulesEngine, IRetrieveTokenInfo retrieveTokenInfo) :base(token, rulesEngine, retrieveTokenInfo)
        {
        }

        public override void DoWork()
        {
            if (!CanContinue())
                Console.WriteLine("Failed Validation.");

            Console.WriteLine("Process Request.");
        }
    }
}
