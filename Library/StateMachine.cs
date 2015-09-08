using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.Exceptions;
using Library.Callbacks;

namespace Library
{
    public delegate void StateCallback(string currentState);
    public delegate void ArcCallback(string sourceState, string targetState);

    public class StateMachine
    {
        private Dictionary<string, State> _statesMap;
        private string _postedState;

        public bool Transiting;
        public State CurrentState { get; private set; }
        private State this[string stateName]
        {
            get
            {
                if (!_statesMap.ContainsKey(stateName))
                {
                    return null;
                }

                return _statesMap[stateName];
            }
        }


        public StateMachine()
        {
            _statesMap = new Dictionary<string, State>();
            _postedState = string.Empty;
            Transiting = false;

            CurrentState = null;
        }

        public void AddState(string stateName)
        {
            if (this[stateName] != null)
            {
                throw new StateMachineException(ErrorCodes.AlreadyPresentState, stateName);
            }

            State s = new State(stateName);
            _statesMap[stateName] = s;
        }

        public void AddArc(string source, string target)
        {
            State sourceState = this[source];

            if (sourceState == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, source);
            }

            State targetState = this[target];

            if (targetState == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, target);
            }

            sourceState.AddArc(targetState);
        }

        public void AddEnterStateCallback(string targetStateName, StateCallback method)
        {
            State targetState = this[targetStateName];

            if (targetState == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, targetStateName);
            }

            targetState.AddStateCallback(method, true);
        }

        public void AddExitStateCallback(string targetStateName, StateCallback method)
        {
            State targetState = this[targetStateName];

            if (targetState == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, targetStateName);
            }

            targetState.AddStateCallback(method, false);
        }

        public void AddTransitArcCallback(string source, string target, ArcCallback method)
        {
            State s = this[source];

            if (s == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, source);
            }

            s.AddTransitArcCallback(target, method);
        }

        public void GoToState(string stateName)
        {
            if (Transiting)
            {
                if (_postedState.Length != 0)
                {
                    throw new StateMachineException(ErrorCodes.PostedStateAreadySet, string.Empty);
                }

                _postedState = stateName;
                return;
            }

            try
            {
            PostedStateRestart:
                Transiting = true;

                State target = this[stateName];

                if (target == null)
                {
                    throw new StateMachineException(ErrorCodes.UnknownState, stateName);
                }

                if (CurrentState != null)
                {
                    Arc arc = CurrentState[target];

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

                if (_postedState.Length != 0)
                {
                    stateName = _postedState;
                    _postedState = string.Empty;
                    goto PostedStateRestart; //tail recursion
                }
            }
            catch
            {
                Transiting = false;
                throw;
            }
        }
    }
}
