using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Api.Enums
{
	public enum LimiterRule
	{
		BasedOnTime,
		BasedOnTimeSinceLastCall
	}
}
