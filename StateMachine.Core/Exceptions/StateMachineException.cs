namespace StateMachine.Core.Exceptions
{
    public class StateMachineException<TState> : Exception
    {
        public ErrorCodes ErrorCode { get; }

        public StateMachineException(ErrorCodes ec)
            : this(ec, GetErrorMessage(ec))
        {
        }

        public StateMachineException(ErrorCodes ec, TState currentState)
            : this(ec, GetErrorMessage(ec, currentState))
        {
        }

        public StateMachineException(ErrorCodes ec, string msg)
            : base(msg)
        {
            ErrorCode = ec;
        }

        public StateMachineException(ErrorCodes ec, TState sourceState, TState targetState)
            : this(ec, GetErrorMessage(ec, sourceState, targetState))
        {
        }

        private static string GetErrorMessage(ErrorCodes errorCode, TState? sourceState = default, TState? targetState = default)
        {
            static string getArcName(TState source, TState target) => $"{source} -> {target}";

            string baseMsg =
                errorCode switch
                {
                    ErrorCodes.StateAlreadyAdded => $"The state {sourceState} is already added",
                    ErrorCodes.StateNotFound => $"No states {sourceState} is found",
                    ErrorCodes.InvalidArc => $"The arc {getArcName(sourceState!, targetState!)} is invalid",
                    ErrorCodes.ArcAlreadyAdded => $"The arc {getArcName(sourceState!, targetState!)} is already added",
                    ErrorCodes.ArcNotFound => $"The arc {getArcName(sourceState!, targetState!)} is not found",
                    ErrorCodes.AlreadyTransiting => "The state machine is already transiting",
                    _ => throw new NotImplementedException()
                };

            return $"[{errorCode}] {baseMsg}";
        }
    }
}
