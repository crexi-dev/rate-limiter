using RateLimiter.Contracts;
using RateLimiterMy.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterMy.Models
{
    public class Request : IRequest
    {
        private string _Controler;
        private string _Method;
        private string _Token;
        private string _IP;
        private Location _Location;
        private DateTime _Time;

        public string Controler { get => _Controler; }
        public string Method { get => _Method; }
        public string Token { get => _Token; }
        public string IP { get => _IP; }
        public Location Location { get => _Location; }
        public DateTime Time { get => _Time; }


        public Request(string controler, string method, string token, ITimeService date, Location location) : this(controler, method, token, date)
        {
            _Location = location;
        }

        public Request(string controler, string method, string token, ITimeService timeService, string ip) : this(controler, method, token, timeService)
        {
            _IP = ip;
            _Location = GetLocationByIp(ip);
        }

        private Request(string controler, string method, string token, ITimeService timeService)
        {
            _Controler = controler;
            _Method = method;
            _Token = token;
            _Time = timeService.Now;
        }

        /// <summary>
        /// Stub algorithm for determining location by IP 
        /// </summary>
        /// <param name="ip">IP Address</param>
        /// <returns>Location</returns>
        private Location GetLocationByIp(string ip)
        {
            if (ip.StartsWith("1")) return Location.US;
            if (ip.StartsWith("2")) return Location.JP;
            if (ip.StartsWith("3")) return Location.RU;

            return Location.EU;
        }
    }
}
