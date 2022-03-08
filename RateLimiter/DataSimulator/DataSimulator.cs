using System;
using System.Collections.Generic;

namespace RateLimiter
{
    public static class DataSimulator
    {
        public static Dictionary<string, List<DateTime>> UserLogData = new Dictionary<string, List<DateTime>>();
        
        public static List<DateTime> GetUserApiLogTimes(string userToken, string apiName)
        { 
            if (UserLogData.TryGetValue($"{userToken}:{apiName}", out List<DateTime>? userApiLogTimes))
            {
                return userApiLogTimes;
            }
            else
            {
                return new List<DateTime>();
            }
        }

        public static void SaveOrUpdateApiLog(string userToken, string apiName, List<DateTime> userApiLogTimes)
        {
            UserLogData[$"{userToken}:{apiName}"] = userApiLogTimes;
        }
        
    }
}
