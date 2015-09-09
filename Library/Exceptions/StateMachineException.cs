﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Exceptions
{
    public class StateMachineException : Exception
    {
        public ErrorCodes ErrorCode { get; private set; }

        public StateMachineException(ErrorCodes ec)
            : this(ec, "Unknown error from StateMachine", null)
        {
        }

        public StateMachineException(ErrorCodes ec, string message)
            : this(ec, message, null)
        {
        }

        public StateMachineException(ErrorCodes ec, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = ec;
        }

        public static string MakeArcName<TState>(TState source, TState target)
        {
            return string.Format("{0} -> {1}", source, target);
        }
    }
}
