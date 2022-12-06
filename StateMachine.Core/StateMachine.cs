using StateMachine.Core.EventArgs;
using StateMachine.Core.Exceptions;

namespace StateMachine.Core
{
    public class StateMachine<TState> where TState : notnull
    {
        private readonly Dictionary<TState, StateContainer<TState>> _statesMap = new();
        private StateContainer<TState> this[TState state]
        {
            get
            {
                if (!_statesMap.ContainsKey(state))
                {
                    throw new StateMachineException<TState>(ErrorCodes.UnknownState, state);
                }

                return _statesMap[state];
            }
        }

        public bool IsTransiting { get; private set; }
        public StateContainer<TState>? CurrentState { get; private set; }

        public event EventHandler<TransitingEventArgs<TState>>? Transiting = default;
        public event EventHandler<TransitedEventArgs<TState>>? Transited = default;

        protected virtual void OnTransiting(TransitingEventArgs<TState> e) => Transiting?.Invoke(this, e);

        protected virtual void OnTransited(TransitedEventArgs<TState> e) => Transited?.Invoke(this, e);

        public StateMachine()
        {
            IsTransiting = false;
            CurrentState = null;
        }

        public void AddState(TState stateName)
        {
            if (_statesMap.ContainsKey(stateName))
            {
                throw new StateMachineException<TState>(ErrorCodes.AlreadyPresentState, stateName);
            }

            StateContainer<TState> state = new(stateName);
            _statesMap[stateName] = state;
        }

        public void AddTransition(TState source, TState target)
        {
            StateContainer<TState> sourceState = this[source];
            StateContainer<TState> targetState = this[target];
            sourceState.AddTransition(targetState);
        }

        public void AddEnterStateCallback(TState targetStateName, Action<TState> method)
        {
            StateContainer<TState> targetState = this[targetStateName];
            targetState.AddStateCallback(method, true);
        }

        public void AddExitStateCallback(TState targetStateName, Action<TState> method)
        {
            StateContainer<TState> targetState = this[targetStateName];
            targetState.AddStateCallback(method, false);
        }

        public void AddTransitionCallback(TState source, TState target, Action<TState, TState> method)
        {
            StateContainer<TState> state = this[source];
            state.AddTransitionCallback(target, method);
        }

        public void GoToState(TState stateName)
        {
            try
            {
                if (IsTransiting)
                {
                    throw new StateMachineException<TState>(ErrorCodes.AlreadyTransiting, stateName);
                }

                IsTransiting = true;
                OnTransiting(new TransitingEventArgs<TState>(stateName));
                StateContainer<TState> target = this[stateName];

                if (CurrentState != null)
                {
                    Transition<TState> transition = CurrentState[target];

                    CurrentState.CallExitCallbacks();
                    transition.CallTransitionCallbacks();
                }

                CurrentState = target;
                target.CallEnterCallbacks();
                IsTransiting = false;
                OnTransited(new TransitedEventArgs<TState>(CurrentState.State));
            }
            catch
            {
                IsTransiting = false;
                throw;
            }
        }
    }
}
