using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class RateLimiterConfigModel
    {

        public RuleSettingModel DefaultRule { get; set; }
        public List<RuleSettingModel> Rules { get; set; }
    }

}
