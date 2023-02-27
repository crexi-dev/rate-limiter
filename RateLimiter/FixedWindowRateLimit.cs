using System;
using System.Collections.Generic;
using System.Linq;

public class FixedWindowRateLimit : IRule
{
    public TimeSpan Window { get; set; }
    public int PermitLimit { get; set; }

    public bool Validate(List<DateTime> attempts)
    {
        var maxDate = attempts.Max();
        return attempts.Count(x => x >= maxDate - Window) <= PermitLimit;
    }
}