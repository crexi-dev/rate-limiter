using System;

namespace RateLimits.History
{
    public class HistoryModel
    {
        public HistoryModel(DateTime date, string region)
        {
            Date = date;
            Region = region;
        }

        public  DateTime Date { get; }
        public string Region { get; }
    }
}
