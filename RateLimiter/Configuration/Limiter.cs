using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Configuration
{
    public class Limiter : ConfigurationSection
    {
        [IntegerValidator(MinValue = 1)]
        [ConfigurationProperty("Count", DefaultValue = 1, IsRequired = false)]
        public int Count {
            get { return (int)this["Count"]; }
            //set { this["Count"] = value; }
        }


        [IntegerValidator(MinValue = 1)]
        [ConfigurationProperty("SecondsSpan", DefaultValue = 1, IsRequired = false)]
        public int SecondsSpan {
            get { return (int)this["SecondsSpan"]; }
            //set { this["SecondsSpan"] = value; }
        }


        [ConfigurationProperty("Title", IsRequired = true)]
        public string Title {
            get { return (string)this["Title"]; }
            //set { this["Title"] = value; }
        }
    }
}
