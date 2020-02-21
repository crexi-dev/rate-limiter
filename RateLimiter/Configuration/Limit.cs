using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Configuration
{
    public class Limit : ConfigurationSection
    {
        [ConfigurationProperty("Token", IsRequired = true)]
        public string Token
        {
            get { return (string)this["Token"]; }
        }

        [ConfigurationProperty("Limits", IsRequired = true)]
        public List<string> Limits
        {
            get { return (List<string>)this.Properties.Cast<ConfigurationProperty>().Select(t => (string)t.).ToList(); }
        }
    }
}
