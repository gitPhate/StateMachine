using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.EventArgs
{
    public class TransitingEventArgs<TState> : AbstractStateEventArgs<TState>
    {
        public TransitingEventArgs(TState state)
            : base(state)
        {
        }
    }
}
