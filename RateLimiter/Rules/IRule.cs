using RateLimits.History;
using System;
using System.Collections.Generic;

namespace RateLimits.Rules
{
    public interface IRule
    {
        bool Execute(IEnumerable<HistoryModel> history, string userRegion);
    }
}
