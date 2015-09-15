using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine.Callbacks
{
    public class ArcCallbackInvoker
    {
        public ArcCallback Callback { get; private set; }

        public ArcCallbackInvoker(ArcCallback callback)
        {
            Callback = callback;
        }

        public void Invoke(string source, string target)
        {
            Callback(source, target);
        }
    }
}
