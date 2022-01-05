using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter
{
    public class RateLimiters
    {
        private static Dictionary<int, List<Token>> conditionPerUser = new Dictionary<int, List<Token>>();

        public bool CheckRateLimit(int user, Rules rules)
        {
            var checkDate = DateTime.Now;
            if (conditionPerUser.ContainsKey(user))
            {
                if (rules.HasFlag(Rules.LastAccessTime))
                {
                    if (conditionPerUser[user].Select(x => x.LastAccessTime).Last() > checkDate.AddSeconds(-5))
                    {
                        return false;
                    }
                }
                if (rules.HasFlag(Rules.AccessCounter))
                {
                    // last five seconds access check
                    if (conditionPerUser[user].Where(x => x.LastAccessTime > checkDate.AddMinutes(-1)).Count() > 5)
                    {
                        return false;
                    }
                }

                conditionPerUser[user].Add(new Token { LastAccessTime = checkDate });
            }
            else
            {
                conditionPerUser.Add(user, new List<Token> { new Token { LastAccessTime = checkDate } });
            }

            return true;
        }
    }
}
