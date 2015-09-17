using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMachine.Callbacks;
using StateMachine.Exceptions;

namespace StateMachine
{
    public class State
    {
        private Dictionary<string, Arc> _arcsMap;
        private List<StateCallbackInvoker> _enterCallbacks;
        private List<StateCallbackInvoker> _exitCallbacks;

        public string Name { get; private set; }

        public Arc this[State s]
        {
            get
            {
                if (!_arcsMap.ContainsKey(s.Name))
                {
                    return null;
                }

                return _arcsMap[s.Name];
            }
        }

        public State(string stateName)
        {
            Name = stateName;

            _arcsMap = new Dictionary<string, Arc>();
            _enterCallbacks = new List<StateCallbackInvoker>();
            _exitCallbacks = new List<StateCallbackInvoker>();
        }

        public void AddArc(State s)
        {
            if (_arcsMap.ContainsKey(s.Name))
            {
                throw new StateMachineException(ErrorCodes.AlreadyPresentArc, StateMachineException.MakeArcName(Name, s.Name));
            }

            Arc a = new Arc(this, s);
            _arcsMap[s.Name] = a;
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
            if (!_arcsMap.ContainsKey(target))
            {
                throw new StateMachineException(ErrorCodes.UnknownArc, StateMachineException.MakeArcName(Name, target));
            }

            Arc a = _arcsMap[target];

            a.AddTransitionCallback(method);
        }

        public void CallEnterCallbacks()
        {
            foreach (StateCallbackInvoker sci in _enterCallbacks)
            {
                sci.Invoke(Name);
            }
        }

        public void CallExitCallbacks()
        {
            foreach (StateCallbackInvoker sci in _exitCallbacks)
            {
                sci.Invoke(Name);
            }
        }
    }
}
