using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RateLimiter.Models;

namespace RateLimiter.Machine;

internal class RuleStateMachine<T> : IRuleStateMachine<T>
{
    private readonly ImmutableDictionary<State, Transition<T>[]> _transitions;

    private readonly ILogger _logger;

    internal RuleStateMachine(ImmutableDictionary<State, Transition<T>[]> transitions, ILogger logger)
    {
        _transitions = transitions;
        _logger = logger;
    }

    public async Task<bool> RunAsync(T input, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Machine start");
        State state = await StepAsync(input, State.InitState, cancellationToken);
        do
        {
            if (state.IsFinite)
            {
                _logger.LogDebug("Machine finish");
                return true;
            }

            if (state == State.FailureState)
            {
                _logger.LogWarning($"Machine is in failure state. Data: {input}");
                return false;
            }

            state = await StepAsync(input, state, cancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"New state: {state}");
        } while (true);
    }
    
    internal async Task<State> StepAsync(T input, State currentState, CancellationToken cancellationToken)
    {
        foreach (var transition in _transitions[currentState])
        {
            if (await transition.TryExecuteAsync(input, cancellationToken).ConfigureAwait(false))
                return transition.NextState;
        }

        return State.FailureState;
    }
}