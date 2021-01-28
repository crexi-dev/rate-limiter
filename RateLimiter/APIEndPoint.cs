using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class APIEndPoint
    {
        String EndpointId;
        private String Token;
        private DateTime CurrentRequestTime;
        private DateTime LastRequestTime;
        private Int32 NumberOfRequests;
        private DateTime LastSpanTime;

        public APIEndPoint(String EndpointId, String Token, DateTime RequestTime)
        {
            this.EndpointId = EndpointId;
            this.Token = Token;
            this.CurrentRequestTime = RequestTime;
            this.LastRequestTime = DateTime.MinValue;
            this.NumberOfRequests = 1;
            this.LastSpanTime = RequestTime;
        }

        public void Update(DateTime RequestTime)
        {
            this.LastRequestTime = this.CurrentRequestTime;
            this.CurrentRequestTime = RequestTime;
            this.NumberOfRequests++;
        }

        public Boolean IsAfterTimeSpan(Double TimeSpan)
        {
            DateTime TimeSpanSinceLastCall = this.LastRequestTime.AddSeconds(TimeSpan);
            Int32 ComparisonResult = TimeSpanSinceLastCall.CompareTo(this.CurrentRequestTime);

            return ComparisonResult <= 0 ? true : false;
        }

        public Boolean IsWithinRequestsPerTimeSpan(Int32 TimeSpan, Int32 NumberOfRequestsAllowed)
        {
            this.ResetNumberOfRequestsPerTimeSpan(TimeSpan);
            return NumberOfRequestsAllowed >= this.NumberOfRequests ? true : false;
        }

        private void ResetNumberOfRequestsPerTimeSpan(Int32 TimeSpan)
        {
            DateTime SpanTime = this.LastSpanTime.AddSeconds(TimeSpan);

            if (SpanTime.CompareTo(this.CurrentRequestTime) <= 0)
            {
                this.NumberOfRequests = 1;
                this.LastSpanTime = this.CurrentRequestTime;
            }
        }
    }
}
