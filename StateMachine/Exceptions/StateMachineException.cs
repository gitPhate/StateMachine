using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine.Exceptions
{
    public class StateMachineException<TState> : Exception
    {
        public ErrorCodes ErrorCode { get; private set; }

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

        public StateMachineException(ErrorCodes ec, TState currentState, Exception innerException)
            : this(ec, currentState.ToString(), innerException)
        {
            ErrorCode = ec;
        }

        private StateMachineException(ErrorCodes ec, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = ec;
        }

        public static string MakeArcName(TState source, TState target)
        {
            return string.Format("{0} -> {1}", source, target);
        }
    }
}
