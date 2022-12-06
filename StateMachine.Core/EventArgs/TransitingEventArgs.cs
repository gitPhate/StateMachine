namespace StateMachine.Core.EventArgs
{
    public class TransitingEventArgs<TState> : AbstractStateEventArgs<TState>
    {
        public TransitingEventArgs(TState state)
            : base(state)
        {
        }
    }
}
