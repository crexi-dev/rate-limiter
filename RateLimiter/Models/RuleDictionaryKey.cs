using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class RuleDictionaryKey
    {
        public string Token { get; set; }

        public string Rule { get; set; }

        public override int GetHashCode()
        {
            return Token.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            var other = obj as RuleDictionaryKey;

            if (other == null)
                return false;

            return this.Rule.Equals(other.Rule) && this.Token.Equals(other.Token);
        }
    }
}
