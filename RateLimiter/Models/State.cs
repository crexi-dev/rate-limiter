using System;

namespace RateLimiter.Models;

internal readonly record struct State(bool IsFinite, byte Id)
{
    public static readonly State InitState = new State(false, 1);
    public static readonly State FailureState = new State(false, 0);
}