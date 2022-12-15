using StateMachine.Core.Exceptions;

namespace StateMachine.Core
{
    public class Arc<TState> where TState : notnull
    {
        private readonly List<Func<TState, TState, Task>> _arcCallbacks = new();

        public State<TState> Source { get; }
        public State<TState> Target { get; }

        public Arc(State<TState> source, State<TState> target)
        {
            Source = source;
            Target = target;

            if (Source == null || Target == null)
            {
                throw new StateMachineException<TState>(ErrorCodes.InvalidArc, source.Value, target.Value);
            }
        }

        public void AddCallback(Func<TState, TState, Task> predicate) => _arcCallbacks.Add(predicate);

        public async Task InvokeAllCallbacksAsync()
        {
            foreach (Func<TState, TState, Task> callback in _arcCallbacks)
            {
                try
                {
                    await callback(Source.Value, Target.Value).ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
