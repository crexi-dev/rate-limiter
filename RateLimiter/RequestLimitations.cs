using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public static class Limitations
    {
        public static Boolean IsSatisfied(IRequestLimitation Limitation, APIRequest Request)
        {
            return Limitation.IsSatisfiedBy(Request);
        }
    }

    public class TimeSpanSinceLastCallLimitation : IRequestLimitation
    {
        private readonly Int32 TimeSpan;

        public TimeSpanSinceLastCallLimitation(Int32 TimeSpan)
        {
            this.TimeSpan = TimeSpan;
        }

        public bool IsSatisfiedBy(APIRequest Request)
        {
            return Request.IsAfterTimeSpan(TimeSpan);
        }
    }

    public class XRequestsPerTimeSpanLimitation : IRequestLimitation
    {
        private readonly Int32 TimeSpan;
        private readonly Int32 NumberOfRequestsAllowed;

        public XRequestsPerTimeSpanLimitation(Int32 TimeSpan, Int32 NumberOfRequestsAllowed)
        {
            this.TimeSpan = TimeSpan;
            this.NumberOfRequestsAllowed = NumberOfRequestsAllowed;
        }

        public bool IsSatisfiedBy(APIRequest Request)
        {
            return Request.IsWithinRequestsPerTimeSpan(this.TimeSpan, this.NumberOfRequestsAllowed);
        }
    }

    public class TimeSpanSinceLastCallLimitationException : Exception
    {
        public TimeSpanSinceLastCallLimitationException() : base ("Too many requests per time span.")
        {

        }

        public TimeSpanSinceLastCallLimitationException(String Message) : base(Message)
        {
            
        }
    }

    public class XRequestsPerTimeSpanException : Exception
    {
        public XRequestsPerTimeSpanException() : base("Request limit per current time span has been reached.")
        {

        }

        public XRequestsPerTimeSpanException(String Message) : base(Message)
        {

        }
    }
}
