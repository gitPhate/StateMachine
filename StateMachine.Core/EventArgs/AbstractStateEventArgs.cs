namespace StateMachine.Core.EventArgs
{
    public class AbstractStateEventArgs<TState> : System.EventArgs
    {
        public TState State { get; private set; }

        public AbstractStateEventArgs(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            State = state;
        }
    }
}
