using StateMachine.Core.Exceptions;

namespace StateMachine.Core
{
    public class State<TState> where TState : notnull
    {
        private readonly Dictionary<TState, Arc<TState>> _arcsMap = new();
        private readonly List<Func<TState, Task>> _enterCallbacks = new();
        private readonly List<Func<TState, Task>> _exitCallbacks = new();

        public TState Value { get; }

        public Arc<TState> this[TState state]
        {
            get
            {
                if (!_arcsMap.ContainsKey(state))
                {
                    throw new StateMachineException<TState>(ErrorCodes.ArcNotFound, Value, state);
                }

                return _arcsMap[state];
            }
        }

        public State(TState stateName)
        {
            Value = stateName;
        }

        public void AddArc(State<TState> state)
        {
            if (_arcsMap.ContainsKey(state.Value))
            {
                throw new StateMachineException<TState>(ErrorCodes.ArcAlreadyAdded, Value, state.Value);
            }

            Arc<TState> arc = new(this, state);
            _arcsMap[state.Value] = arc;
        }

        public void AddStateCallback(Func<TState, Task> predicate, bool isEnterCallback)
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

        public void AddArcCallback(TState target, Func<TState, TState, Task> predicate)
        {
            Arc<TState> transition = this[target];
            transition.AddCallback(predicate);
        }

        public async Task InvokeEnterCallbacksAsync()
        {
            foreach (Func<TState, Task> callback in _enterCallbacks)
            {
                try
                {
                    await callback(Value).ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }
        }

        public async Task InvokeExitCallbacksAsync()
        {
            foreach (Func<TState, Task> callback in _exitCallbacks)
            {
                try
                {
                    await callback(Value).ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
