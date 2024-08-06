using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Storage
{
    public class Storage : IStorage
    {
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
                    DateTime.Now,
                };
                return Visits.TryAdd(SessionID, visit);
            }
            else
            {
                // add new log to the visits list
                Visits[SessionID].Add(DateTime.Now);
                return true;
            }
        }

        public bool Exist(Guid SessionID)
        {
            return Visits.Keys.Contains(SessionID);
        }


        /// this should be config driven
        /// take last available 10 minutes
        public List<DateTime>? Get(Guid SessionID)
        {
            if (Exist(SessionID))
                return Visits[SessionID].Where(e => e > DateTime.Now.AddMinutes(-5)).ToList();
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
