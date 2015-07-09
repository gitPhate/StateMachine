using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.Callbacks;
using Library.Exceptions;

namespace Library
{
    public class State
    {
        private Dictionary<string, Arc> _mapArcs;
        private List<StateCallbackInvoker> _enterCallbacks;
        private List<StateCallbackInvoker> _exitCallbacks;

        public string Name { get; private set; }

        public Arc this[State s]
        {
            get
            {
                if (!_mapArcs.ContainsKey(s.Name))
                {
                    return null;
                }

                return _mapArcs[s.Name];
            }
        }

        public State(string sName)
        {
            Name = sName;

            _mapArcs = new Dictionary<string, Arc>();
            _enterCallbacks = new List<StateCallbackInvoker>();
            _exitCallbacks = new List<StateCallbackInvoker>();
        }

        public void AddArc(State s)
        {
            if (_mapArcs.ContainsKey(s.Name))
            {
                throw new StateMachineException(ErrorCodes.AlreadyPresentArc, StateMachineException.MakeArcName(Name, s.Name));
            }

            Arc a = new Arc(this, s);
            _mapArcs[s.Name] = a;
        }

        public void AddStateCallback(StateCallback method, bool enter)
        {
            StateCallbackInvoker sci = new StateCallbackInvoker(method);

            if (enter)
            {
                _enterCallbacks.Add(sci);
            }
            else
            {
                _exitCallbacks.Add(sci);
            }
        }

        public void AddTransitArcCallback(string target, ArcCallback method)
        {
            if (!_mapArcs.ContainsKey(target))
            {
                throw new StateMachineException(ErrorCodes.UnknownArc, StateMachineException.MakeArcName(Name, target));
            }

            Arc a = _mapArcs[target];

            a.AddTransitArcCallback(method);
        }

        public void CallEnterCallbacks()
        {
            foreach (StateCallbackInvoker sci in _enterCallbacks)
            {
                sci.Invoke(Name, true);
            }
        }

        public void CallExitCallbacks()
        {
            foreach (StateCallbackInvoker sci in _exitCallbacks)
            {
                sci.Invoke(Name, false);
            }
        }
    }
}
