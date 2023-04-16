using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Machine;

public static class RuleStateMachineBuilder
{
    public static IRuleStateMachine<T> Build<T>(IEnumerable<IRule<T>> rules, ILogger logger)
    {
        var transitions = new Dictionary<State, Transition<T>[]>();
        logger.LogDebug("Machine build start");
        var state = State.InitState;
        foreach (IRule<T> rule in rules)
        {
            var nextState = GetNextState(state);
            transitions[state] = new [] { new Transition<T>(rule, nextState) };
            logger.LogDebug($"Transition for {rule.Description} to {nextState} added");
            state = nextState;
        }
        transitions[state] = new []{ new Transition<T>(new AlwaysTrueRule<T>(), GetFinalState(state)) };
        
        var stateMachine = new RuleStateMachine<T>(transitions.ToImmutableDictionary(), logger);
        logger.LogDebug("Machine build completed");
        return stateMachine;
    }

    private static State GetNextState(State state, bool isFinite = false) => new State(isFinite, (byte)(state.Id + 1));
    private static State GetFinalState(State state) => GetNextState(state, true);
}