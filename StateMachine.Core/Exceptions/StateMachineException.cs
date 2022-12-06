namespace StateMachine.Core.Exceptions
{
    public class StateMachineException<TState> : Exception
    {
        public ErrorCodes ErrorCode { get; }

        public StateMachineException(ErrorCodes ec)
            : this(ec, "Unknown error from StateMachine", null)
        {
        }

        public StateMachineException(ErrorCodes ec, TState currentState)
            : this(ec, currentState, null)
        {
        }

        public StateMachineException(ErrorCodes ec, string message)
            : this(ec, message, null)
        {
        }

        public StateMachineException(ErrorCodes ec, TState sourceState, TState targetState)
            : this(ec, MakeArcName(sourceState, targetState), null)
        {
        }

        public StateMachineException(ErrorCodes ec, TState currentState, Exception? innerException)
            : this(ec, currentState?.ToString() ?? string.Empty, innerException)
        {
            ErrorCode = ec;
        }

        private StateMachineException(ErrorCodes ec, string message, Exception? innerException)
            : base(message, innerException)
        {
            ErrorCode = ec;
        }

        public static string MakeArcName(TState source, TState target) => $"{source} -> {target}";
    }
}
