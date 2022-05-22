using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces.Rules
{
    public interface IRule
    {
        bool Evaluate(RuleSettingModel ruleData, HttpContext httpContext);
    }
}
