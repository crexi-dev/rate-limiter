using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Limiter
{
    public class LimitResolver
    {
        private Dictionary<string, Limit> _limits = new Dictionary<string, Limit>();
        private Dictionary<string, TokenLimits> _tokenLimits = new Dictionary<string, TokenLimits>();

        public static LimitResolver Instance = new LimitResolver();

        private LimitResolver()
        {
        }

        public void AddLimit(string name, int delay, int count)
        {
            lock(_limits)
            {
                if (!_limits.ContainsKey(name))
                    _limits.Add(name, new Limit(delay, count));
            }
        }

        public void AddTokenLimit(string token, string limitName)
        {
            lock (_limits)
            {
                lock (_tokenLimits)
                {
                    if (!_limits.ContainsKey(limitName))
                        return;

                    if (!_tokenLimits.ContainsKey(token))
                    {
                        _tokenLimits.Add(token, new TokenLimits());
                    }

                    _tokenLimits[token].AddLimit(limitName, _limits[limitName]);
                }
            }
        }

        public void DeleteTokenLimit(string token, string limitName)
        {
            lock(_limits)
            {
                lock(_tokenLimits)
                {
                    if (_tokenLimits.ContainsKey(token))
                    {
                        _tokenLimits[token].RemoveLimit(limitName);
                        if(!_tokenLimits[token].Any())
                            _tokenLimits.Remove(token);
                    }
                }
            }
        }

        public bool NewQuery(string token)
        {
            lock (_tokenLimits)
            {
                return (_tokenLimits.ContainsKey(token) ? _tokenLimits[token].NewRequest() : true);
            }
        }
    }
}
