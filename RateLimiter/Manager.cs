using RateLimiter.Interfaces;
using System.Collections.Generic;

namespace RateLimiter
{
    public class Manager
    {
        private readonly IDictionary<string, Client> _values;

        public Manager(IDictionary<string, Client> values)
        {
            _values = values;
        }

        public IEvaluate GetEvaluater(string token)
        {
            return _values[token].GetEvaluator();
        }
    }
}
