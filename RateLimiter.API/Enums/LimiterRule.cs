using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Enums
{
	public enum LimiterRule
	{
		BasedOnTime,
		BasedOnTimeSinceLastCall
	}
}
