using RateLimiter.Configs;
using RateLimiter.Models;
using RateLimiter.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Storage
{
    public class Storage : IStorage
    {

        IRateLimiterConfigs config;
        IDateTimeService dateTime;

        public Storage(IRateLimiterConfigs _config, IDateTimeService _dateTime)
        {
            config = _config;
            dateTime = _dateTime;
        }


        /// <summary>
        /// historical logs for user access
        /// </summary>
        private static ConcurrentDictionary<Guid, List<DateTime>> Visits = new();

        /// log a visit to the shared cache
        public bool AddOrAppend(Guid SessionID)
        {
            /// new entry
            if (!Exist(SessionID))
            {
                List<DateTime> visit = new List<DateTime>()
                {
                    dateTime.GetCurrentTime()
                };
                return Visits.TryAdd(SessionID, visit);
            }
            else
            {
                // add new log to the visits list
                Visits[SessionID].Add(dateTime.GetCurrentTime());
                return true;
            }
        }

        public bool Exist(Guid SessionID)
        {
            return Visits.Keys.Contains(SessionID);
        }


        /// this should be config driven
        /// ex: take last available logs for the past x seconds (based on the configs)
        public List<DateTime>? Get(Guid SessionID)
        {
            if (Exist(SessionID))
            {
                ConfigValues? conVals = config.BindConfig();
                int? confiSeconds = int.Parse(conVals?.TimeFrame?.ToString());
                confiSeconds *= -1; // get negative of that value
                return Visits[SessionID].Where(e => e > DateTime.Now.AddSeconds(confiSeconds.Value)).ToList();
            }
            else
                return null;
        }

        /// remove an entry (if needed)
        /// this should be config driven
        public bool Remove(string password, Guid id)
        {
            if (password == "customPasscode")
                return Visits.TryRemove(id, out _);

            return false;
        }
    }
}
