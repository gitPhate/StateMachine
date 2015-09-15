using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine.CallbackInvokers
{
    public class TransitionCallbackInvoker<TState>
    {
        public ArcCallback<TState> Callback { get; private set; }

        public TransitionCallbackInvoker(ArcCallback<TState> callback)
        {
            Callback = callback;
        }

        public void Invoke(TState source, TState target)
        {
            Callback(source, target);
        }
    }
}
