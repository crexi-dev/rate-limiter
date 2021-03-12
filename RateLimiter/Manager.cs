using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;

namespace RateLimiter
{
    public class Manager
    {
        private readonly IDictionary<string, Client> _values;

        public Manager(IDictionary<string, Client> values)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public IEvaluate GetEvaluater(string token)
        {
            if (_values.ContainsKey(token))
            {
                return _values[token].GetEvaluator();
            }

            return NullableResource.Instance;
        }
    }
}
