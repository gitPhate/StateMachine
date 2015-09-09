using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Invokers
{
    public class TransitionCallbackInvoker<TState>
    {
        public TransitionCallback<TState> Callback { get; private set; }

        public TransitionCallbackInvoker(TransitionCallback<TState> callback)
        {
            Callback = callback;
        }

        public void Invoke(TState source, TState target)
        {
            Callback(source, target);
        }
    }
}
