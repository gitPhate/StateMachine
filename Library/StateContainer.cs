using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.Invokers;
using Library.Exceptions;

namespace Library
{
    public class StateContainer<TState>
    {
        private Dictionary<TState, Transition<TState>> _transitionsMap;
        private IList<StateCallbackInvoker<TState>> _enterCallbacks;
        private IList<StateCallbackInvoker<TState>> _exitCallbacks;

        public TState Name { get; private set; }

        public Transition<TState> this[StateContainer<TState> state]
        {
            get
            {
                if (!_transitionsMap.ContainsKey(state.Name))
                {
                    return null;
                }

                return _transitionsMap[state.Name];
            }
        }

        public StateContainer(TState stateName)
        {
            Name = stateName;

            _transitionsMap = new Dictionary<TState, Transition<TState>>();
            _enterCallbacks = new List<StateCallbackInvoker<TState>>();
            _exitCallbacks = new List<StateCallbackInvoker<TState>>();
        }

        public void AddTransition(StateContainer<TState> state)
        {
            if (_transitionsMap.ContainsKey(state.Name))
            {
                throw new StateMachineException<TState>(ErrorCodes.AlreadyPresentArc, StateMachineException<TState>.MakeArcName(Name, state.Name));
            }

            var transition = new Transition<TState>(this, state);
            _transitionsMap[state.Name] = transition;
        }

        public void AddStateCallback(StateCallback<TState> method, bool enter)
        {
            var sci = new StateCallbackInvoker<TState>(method);

            if (enter)
            {
                _enterCallbacks.Add(sci);
            }
            else
            {
                _exitCallbacks.Add(sci);
            }
        }

        public void AddTransitionCallback(TState target, TransitionCallback<TState> method)
        {
            if (!_transitionsMap.ContainsKey(target))
            {
                throw new StateMachineException<TState>(ErrorCodes.UnknownArc, StateMachineException<TState>.MakeArcName(Name, target));
            }

            var transition = _transitionsMap[target];

            transition.AddTransitionCallback(method);
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
