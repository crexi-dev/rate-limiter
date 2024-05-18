using RateLimiter.Rules.Constructors;
using System;

namespace RateLimiter.Repositories.Detectors;

public interface IRuleConstructorDetector
{
    IRuleConstructor GetConstructor(Type constructorType);
}
