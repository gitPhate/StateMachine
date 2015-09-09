using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.CallbackInvokers;
using Library.Exceptions;

namespace Library
{
    public class StateContainer<TState>
    {
        private Dictionary<TState, Transition<TState>> _arcsMap;
        private IList<StateCallbackInvoker<TState>> _enterCallbacks;
        private IList<StateCallbackInvoker<TState>> _exitCallbacks;

        public TState Name { get; private set; }

        public Transition<TState> this[StateContainer<TState> s]
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

        public StateContainer(TState stateName)
        {
            Name = stateName;

            _arcsMap = new Dictionary<TState, Transition<TState>>();
            _enterCallbacks = new List<StateCallbackInvoker<TState>>();
            _exitCallbacks = new List<StateCallbackInvoker<TState>>();
        }

        public void AddArc(StateContainer<TState> s)
        {
            if (_arcsMap.ContainsKey(s.Name))
            {
                throw new StateMachineException(ErrorCodes.AlreadyPresentArc, StateMachineException.MakeArcName(Name, s.Name));
            }

            Transition<TState> a = new Transition<TState>(this, s);
            _arcsMap[s.Name] = a;
        }

        public void AddStateCallback(StateCallback<TState> method, bool enter)
        {
            StateCallbackInvoker<TState> sci = new StateCallbackInvoker<TState>(method);

            if (enter)
            {
                _enterCallbacks.Add(sci);
            }
            else
            {
                _exitCallbacks.Add(sci);
            }
        }

        public void AddTransitArcCallback(TState target, ArcCallback<TState> method)
        {
            if (!_arcsMap.ContainsKey(target))
            {
                throw new StateMachineException(ErrorCodes.UnknownArc, StateMachineException.MakeArcName(Name, target));
            }

            Transition<TState> a = _arcsMap[target];

            a.AddTransitionCallback(method);
        }

        public void CallEnterCallbacks()
        {
            foreach (StateCallbackInvoker<TState> sci in _enterCallbacks)
            {
                sci.Invoke(Name);
            }
        }

        public void CallExitCallbacks()
        {
            foreach (StateCallbackInvoker<TState> sci in _exitCallbacks)
            {
                sci.Invoke(Name);
            }
        }
    }
}
