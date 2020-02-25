using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Limiter
{
    public class TokenLimits
    {
        class LimitChecker
        {
            private readonly Limit limit;
            private Queue<DateTime> requests = new Queue<DateTime>();

            public LimitChecker(Limit l)
            {
                limit = l;
            }

            public bool NewRequest(DateTime now)
            {
                lock (requests)
                {
                    while (true)
                    {
                        if (!requests.Any())
                            break;
                        var top = requests.Peek();
                        if (now.Ticks - top.Ticks > limit.Delay * 10000000l)
                            requests.Dequeue();
                        else
                            break;
                    }
                    if (requests.Count < limit.Count)
                    {
                        requests.Enqueue(now);
                        return true;
                    }

                    return false;
                }
            }
        }

        private Dictionary<string, LimitChecker> limits = new Dictionary<string, LimitChecker>();

        public void AddLimit(string name, Limit l)
        {
            lock (limits)
            {
                if (!limits.ContainsKey(name))
                    limits.Add(name, new LimitChecker(l));
            }
        }

        public void RemoveLimit(string name)
        {
            lock (limits)
            {
                if(limits.ContainsKey(name))
                    limits.Remove(name);
            }
        }

        public bool NewRequest()
        {
            DateTime now = DateTime.Now;
            lock(limits)
            {
                return limits.All(t => t.Value.NewRequest(now));
            }
        }

        internal bool Any()
        {
            lock(limits)
            {
                return limits.Any();
            }
        }
    }
}
