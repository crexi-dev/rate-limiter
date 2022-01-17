using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RateLimiter
{
    public class ClientLogsStorage
    {
        private List<ClientLog> Storage;

        public ClientLogsStorage()
        {
            Storage = new List<ClientLog>();
        }

        public void AddLog(string token)
        {
            ClientLog clientlog = new ClientLog(token, DateTime.Now);
            Storage.Add(clientlog);
        }

        public List<ClientLog> GetLogsByToken(string token)
        {
            return Storage.Where(clientlog => clientlog.ClientToken == token).ToList();
        }

        public List<ClientLog> GetLogsByToken(string token, DateTime from)
        {
            return Storage.Where(clientlog => clientlog.ClientToken == token && clientlog.ClientLogTime > from).ToList();
        }

        public int LogQuantityByToken(string token)
        {
            return GetLogsByToken(token).Count;
        }

        public int LogQuantityByDateTime(string token, DateTime from)
        {
            return GetLogsByToken(token, from).Count;
        }
        
        public ClientLog GetLast(string token)
        {
            return Storage.Where(clientlog => clientlog.ClientToken == token).OrderByDescending(clientlog => clientlog.ClientLogTime).FirstOrDefault();
        }

    }

}
