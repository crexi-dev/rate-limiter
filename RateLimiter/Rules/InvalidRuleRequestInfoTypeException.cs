using System;
using System.Runtime.Serialization;

namespace RateLimiter.Rules
{
    [Serializable]
    internal class InvalidRuleRequestInfoTypeException : Exception
    {
        public InvalidRuleRequestInfoTypeException()
        {
        }

        public InvalidRuleRequestInfoTypeException(string? message) : base(message)
        {
        }

        public InvalidRuleRequestInfoTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidRuleRequestInfoTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}