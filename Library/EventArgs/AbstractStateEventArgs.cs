using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.EventArgs
{
    public class AbstractStateEventArgs<TState> : System.EventArgs
    {
        public TState State { get; private set; }

        public AbstractStateEventArgs(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            State = state;
        }
    }
}
