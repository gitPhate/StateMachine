using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine.EventArgs
{
    public class TransitingEventArgs<TState> : AbstractStateEventArgs<TState>
    {
        public TransitingEventArgs(TState state)
            : base(state)
        {
        }
    }
}
