using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Configuration
{
    public class LimitersConfiguration : ConfigurationSection
    {
        private static LimitersConfiguration limitersConfiguration = ConfigurationManager.GetSection("LimitersConfiguration") as LimitersConfiguration;

        [ConfigurationProperty("Limiters", IsRequired = true)]
        private List<Limiter> limiters { get { return (List<Limiter>)this["Limiters"]; } }

        public static Dictionary<string,Limiter> Limiters
        {
            get
            {
                return limitersConfiguration.limiters;
            }
        }

        public static Dictionary<string,Limit> Limits;
    }

    public class BlogSettings : ConfigurationSection
    {
        private static BlogSettings settings
          = ConfigurationManager.GetSection("BlogSettings") as BlogSettings;

        public static BlogSettings Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty("frontPagePostCount"
          , DefaultValue = 20
          , IsRequired = false)]
        [IntegerValidator(MinValue = 1
          , MaxValue = 100)]
        public int FrontPagePostCount
        {
            get { return (int)this["frontPagePostCount"]; }
            set { this["frontPagePostCount"] = value; }
        }


        [ConfigurationProperty("title"
          , IsRequired = true)]
        [StringValidator(InvalidCharacters = "  ~!@#$%^&*()[]{}/;’\"|\\"
          , MinLength = 1
          , MaxLength = 256)]
        public string Title
        {
            get { return (string)this["title"]; }
            set { this["title"] = value; }
        }
    }
}
