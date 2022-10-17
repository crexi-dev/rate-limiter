using RateLimiter.DataModel;
using System.Collections.Generic;

namespace RateLimiter
{
    public static class HistoryDataUtil
    {
        private static List<ClientRequestHistory> _historyData = null;

        public static List<ClientRequestHistory> GetHistoryData()
        {
            if (_historyData == null)
            {
                _historyData = new List<ClientRequestHistory>();
            }
            return _historyData;
        }
    }
}
