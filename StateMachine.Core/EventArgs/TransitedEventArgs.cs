namespace StateMachine.Core.EventArgs
{
    public class TransitedEventArgs<TState> : AbstractStateEventArgs<TState>
    {
        public TransitedEventArgs(TState state)
            : base(state)
        {
        }
    }
}
