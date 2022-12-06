using StateMachine.Core.Exceptions;

namespace StateMachine.Core
{
    public class StateContainer<TState> where TState : notnull
    {
        private readonly Dictionary<TState, Transition<TState>> _transitionsMap = new();
        private readonly List<Action<TState>> _enterCallbacks = new();
        private readonly List<Action<TState>> _exitCallbacks = new();

        public TState State { get; private set; }

        public Transition<TState> this[StateContainer<TState> state]
        {
            get
            {
                if (!_transitionsMap.ContainsKey(state.State))
                {
                    throw new StateMachineException<TState>(ErrorCodes.InvalidTransition, State, state.State);
                }

                return _transitionsMap[state.State];
            }
        }

        public StateContainer(TState stateName)
        {
            State = stateName;
        }

        public void AddTransition(StateContainer<TState> state)
        {
            if (_transitionsMap.ContainsKey(state.State))
            {
                throw new StateMachineException<TState>(ErrorCodes.AlreadyPresentArc, State, state.State);
            }

            Transition<TState> transition = new(this, state);
            _transitionsMap[state.State] = transition;
        }

        public void AddStateCallback(Action<TState> predicate, bool isEnterCallback)
        {
            if (isEnterCallback)
            {
                _enterCallbacks.Add(predicate);
            }
            else
            {
                _exitCallbacks.Add(predicate);
            }
        }

        public void AddTransitionCallback(TState target, Action<TState, TState> predicate)
        {
            if (!_transitionsMap.ContainsKey(target))
            {
                throw new StateMachineException<TState>(ErrorCodes.UnknownArc, State, target);
            }

            Transition<TState> transition = _transitionsMap[target];

            transition.AddTransitionCallback(predicate);
        }

        public void CallEnterCallbacks()
        {
            foreach (Action<TState> sci in _enterCallbacks)
            {
                sci(State);
            }
        }

        public void CallExitCallbacks()
        {
            foreach (Action<TState> sci in _exitCallbacks)
            {
                sci(State);
            }
        }
    }
}
