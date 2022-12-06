using StateMachine.Core.Exceptions;

namespace StateMachine.Core
{
    public class Transition<TState> where TState : notnull
    {
        private readonly List<Action<TState, TState>> _transitCallbacks = new();

        public StateContainer<TState> Source { get; }
        public StateContainer<TState> Target { get; }
        public bool IsUnlinked => Source == null || Target == null;

        public Transition(StateContainer<TState> source, StateContainer<TState> target)
        {
            Source = source;
            Target = target;

            if (IsUnlinked)
            {
                throw new StateMachineException<TState>(ErrorCodes.InvalidArc, source.State, target.State);
            }
        }

        public void AddTransitionCallback(Action<TState, TState> predicate) => _transitCallbacks.Add(predicate);

        public void CallTransitionCallbacks()
        {
            foreach (Action<TState, TState> predicate in _transitCallbacks)
            {
                predicate(Source.State, Target.State);
            }
        }
    }
}
