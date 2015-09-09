using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.Exceptions;
using Library.CallbackInvokers;

namespace Library
{
    public delegate void StateCallback<TState>(TState currentState);
    public delegate void ArcCallback<TState>(TState sourceState, TState targetState);

    public class StateMachine<TState>
    {
        private IDictionary<TState, StateContainer<TState>> _statesMap;

        public bool Transiting;
        public StateContainer<TState> CurrentState { get; private set; }
        private StateContainer<TState> this[TState state]
        {
            get
            {
                if (!_statesMap.ContainsKey(state))
                {
                    return null;
                }

                return _statesMap[state];
            }
        }


        public StateMachine()
        {
            _statesMap = new Dictionary<TState, StateContainer<TState>>();
            Transiting = false;

            CurrentState = null;
        }

        public void AddState(TState stateName)
        {
            if (this[stateName] != null)
            {
                throw new StateMachineException(ErrorCodes.AlreadyPresentState, stateName.ToString());
            }

            StateContainer<TState> s = new StateContainer<TState>(stateName);
            _statesMap[stateName] = s;
        }

        public void AddTransition(TState source, TState target)
        {
            StateContainer<TState> sourceState = this[source];

            if (sourceState == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, source.ToString());
            }

            StateContainer<TState> targetState = this[target];

            if (targetState == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, target.ToString());
            }

            sourceState.AddArc(targetState);
        }

        public void AddEnterStateCallback(TState targetStateName, StateCallback<TState> method)
        {
            StateContainer<TState> targetState = this[targetStateName];

            if (targetState == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, targetStateName.ToString());
            }

            targetState.AddStateCallback(method, true);
        }

        public void AddExitStateCallback(TState targetStateName, StateCallback<TState> method)
        {
            StateContainer<TState> targetState = this[targetStateName];

            if (targetState == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, targetStateName.ToString());
            }

            targetState.AddStateCallback(method, false);
        }

        public void AddTransitionCallback(TState source, TState target, ArcCallback<TState> method)
        {
            StateContainer<TState> s = this[source];

            if (s == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, source.ToString());
            }

            s.AddTransitArcCallback(target, method);
        }

        public void GoToState(TState stateName)
        {
            try
            {
                if (Transiting)
                {
                    return;
                }

                Transiting = true;
                StateContainer<TState> target = this[stateName];

            if (target == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, stateName.ToString());
            }

            if (CurrentState != null)
            {
                Transition<TState> arc = CurrentState[target];

                if (arc == null)
                {
                    throw new StateMachineException(ErrorCodes.InvalidTransition, StateMachineException.MakeArcName(CurrentState.Name, target.Name));
                }

                CurrentState.CallExitCallbacks();
                arc.CallTransitionCallbacks();
            }

            CurrentState = target;
            target.CallEnterCallbacks();
            Transiting = false;
            }
            catch
            {
                Transiting = false;
                throw;
            }
        }
    }
}
