using System;
using System.Runtime.Serialization;

namespace RateLimiter.Guards
{
    [Serializable]
    internal class InvalidRuleRequestInfoType : Exception
    {
        public InvalidRuleRequestInfoType()
        {
        }

        public InvalidRuleRequestInfoType(string? message) : base(message)
        {
        }

        public InvalidRuleRequestInfoType(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidRuleRequestInfoType(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}