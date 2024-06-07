using RateLimiter.Models;
using System;
using System.Collections.Generic;

namespace RateLimiter.Services.Interfaces;

public interface IRequestValidator
{
    IRequestValidator SetNext(IRequestValidator next);
    (bool isAllowed, int? statusCode, string message) Check(string resource, Region? region, IDictionary<DateTime, RateLimitRequestModel> requestHistory);
}