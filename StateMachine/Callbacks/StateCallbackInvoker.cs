using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine.Callbacks
{
    public class StateCallbackInvoker
    {
        public StateCallback Callback { get; private set; }

        public StateCallbackInvoker(StateCallback callback)
        {
            Callback = callback;
        }

        public void Invoke(string state)
        {
            Callback(state);
        }
    }
}
