using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models.Options
{
    public class ActiveProcessorsOptions
    {
        public const string Position = "ActiveProcessors";

        public IList<string> ActiveProcessorNames = new List<string>();
    }
}
