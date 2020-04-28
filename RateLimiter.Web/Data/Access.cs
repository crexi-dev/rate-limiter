using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Data
{
    public class Access
    {

        public Access()
        {
        }

        public Access(string ip, int account, string token, DateTime date)
        {
            IP = ip;
            Account = account;
            Token = token;
            Date = date;
        }

        public long ID { get; set; }
        public string IP { get; set; }
        public int Account { get; set; }
        public string Token { get; set; }
        public DateTime Date { get; set; }

    }
}
