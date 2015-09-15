using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine.Invokers
{
    public class StateCallbackInvoker<TState>
    {
        public StateCallback<TState> Callback { get; private set; }

        public StateCallbackInvoker(StateCallback<TState> callback)
        {
            Callback = callback;
        }

        public void Invoke(TState state)
        {
            Callback(state);
        }
    }
}
