using System;
using System.Runtime.Serialization;

namespace RateLimiter.Repositories
{
    [Serializable]
    internal class InvalidRuleConstructorType : Exception
    {
        public InvalidRuleConstructorType()
        {
        }

        public InvalidRuleConstructorType(string? message) : base(message)
        {
        }

        public InvalidRuleConstructorType(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidRuleConstructorType(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}