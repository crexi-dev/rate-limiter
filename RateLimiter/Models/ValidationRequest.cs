using System;

namespace RateLimiter;

// We don't pass an HTTP request.
// This is a container for request values used in validation.
public class ValidationRequest
{
    public DateTime RequestTime { get; set; }
}