using System;

namespace Core.Models
{
    public class ClientData
    {
        public ClientData(DateTime previousSuccessfullCallTime)
        {
            PreviousSuccessfullCallTime = previousSuccessfullCallTime;
        }

        public ClientData(DateTime previousSuccessfullCallTime, int numberOfRequestsCompletedSuccessfully)
            : this(previousSuccessfullCallTime)
        {
            NumberOfRequestsCompletedSuccessfully = numberOfRequestsCompletedSuccessfully;
        }

        public DateTime PreviousSuccessfullCallTime { get; set; }
        public int NumberOfRequestsCompletedSuccessfully { get; set; }
    }
}
