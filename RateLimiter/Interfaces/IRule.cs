using System;
using System.Collections.Generic;

public interface IRule
{
    bool Validate(List<DateTime> attempts);
}