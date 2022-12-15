using StateMachine.Core.EventArgs;
using StateMachine.Core.Exceptions;

namespace StateMachine.Core
{
    public class StateMachine<TState> where TState : notnull
    {
        private readonly Dictionary<TState, State<TState>> _statesMap = new();
        private State<TState> this[TState state]
        {
            get
            {
                if (!_statesMap.ContainsKey(state))
                {
                    throw new StateMachineException<TState>(ErrorCodes.StateNotFound, state);
                }

                return _statesMap[state];
            }
        }

        public bool IsTransiting { get; private set; }
        public TState CurrentState { get; private set; }

        public event EventHandler<TransitingEventArgs<TState>> Transiting = delegate { };
        public event EventHandler<TransitedEventArgs<TState>> Transited = delegate { };

        protected virtual void OnTransiting(TransitingEventArgs<TState> e) => Transiting?.Invoke(this, e);

        protected virtual void OnTransited(TransitedEventArgs<TState> e) => Transited?.Invoke(this, e);

        public StateMachine(TState currentState)
        {
            IsTransiting = false;
            CurrentState = currentState;
            AddState(currentState);
        }

        public void AddState(TState stateName)
        {
            if (_statesMap.ContainsKey(stateName))
            {
                throw new StateMachineException<TState>(ErrorCodes.StateAlreadyAdded, stateName);
            }

            State<TState> state = new(stateName);
            _statesMap[stateName] = state;
        }

        public void AddArc(TState source, TState target)
        {
            State<TState> sourceState = this[source];
            State<TState> targetState = this[target];
            sourceState.AddArc(targetState);
        }

        public void AddEnterStateCallback(TState targetStateName, Func<TState, Task> callback)
        {
            State<TState> targetState = this[targetStateName];
            targetState.AddStateCallback(callback, true);
        }

        public void AddExitStateCallback(TState targetStateName, Func<TState, Task> callback)
        {
            State<TState> targetState = this[targetStateName];
            targetState.AddStateCallback(callback, false);
        }

        public void AddArcCallback(TState source, TState target, Func<TState, TState, Task> callback)
        {
            State<TState> state = this[source];
            state.AddArcCallback(target, callback);
        }

        public async Task GoToStateAsync(TState targetState)
        {
            try
            {
                if (IsTransiting)
                {
                    throw new StateMachineException<TState>(ErrorCodes.AlreadyTransiting, targetState);
                }

                IsTransiting = true;
                OnTransiting(new TransitingEventArgs<TState>(targetState));

                State<TState> currentStateWrapper = this[CurrentState];
                Arc<TState> arc = currentStateWrapper[targetState];
                await currentStateWrapper.InvokeExitCallbacksAsync().ConfigureAwait(false);
                await arc.InvokeAllCallbacksAsync().ConfigureAwait(false);

                CurrentState = targetState;

                State<TState> targetStateWrapper = this[targetState];
                await targetStateWrapper.InvokeEnterCallbacksAsync().ConfigureAwait(false);

                OnTransited(new TransitedEventArgs<TState>(CurrentState));
            }
            finally
            {
                IsTransiting = false;
            }
        }
    }
}
