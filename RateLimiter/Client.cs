using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;

namespace RateLimiter
{
    public class Client
    {
        private readonly Stack<DateTimeOffset> _request = new Stack<DateTimeOffset>();
        private readonly Func<Stack<DateTimeOffset>, IEvaluate> _evaluater;

        public Client(Func<Stack<DateTimeOffset>, IEvaluate> evaluater)
        {
            _evaluater = evaluater;
        }

        public IEvaluate GetEvaluator()
        {
            return _evaluater(_request);
        }
    }
}
