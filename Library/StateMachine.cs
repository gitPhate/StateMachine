using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.Exceptions;
using Library.Callbacks;

namespace Library
{
    public delegate void StateCallback(string currentState, bool isEnterCallback);
    public delegate void ArcCallback(string sourceState, string targetState);

    public class StateMachine
    {
        private Dictionary<string, State> _mapStates;
        private string _postedState;
        private bool _transiting;

        public State CurrentState { get; private set; }
        public State this[string stateName]
        {
            get
            {
                if (!_mapStates.ContainsKey(stateName))
                {
                    return null;
                }

                return _mapStates[stateName];
            }
        }

        public StateMachine()
        {
            _mapStates = new Dictionary<string, State>();
            _postedState = string.Empty;

            _transiting = false;
            CurrentState = null;
        }

        public void AddState(string stateName)
        {
            if (this[stateName] != null)
            {
                throw new StateMachineException(ErrorCodes.AlreadyPresentState, stateName);
            }

            State s = new State(stateName);
            _mapStates[stateName] = s;
        }

        public void AddArc(string source, string target)
        {
            State sSource = this[source];
            if (sSource == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, source);
            }

            State sTarget = this[target];
            if (sTarget == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, target);
            }

            sSource.AddArc(sTarget);
        }

        public void AddEnterStateCallback(string stateName, StateCallback method)
        {
            State s = this[stateName];
            if (s == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, stateName);
            }

            s.AddStateCallback(method, true);
        }

        public void AddExitStateCallback(string stateName, StateCallback method)
        {
            State s = this[stateName];
            if (s == null)
            {
                throw new StateMachineException(ErrorCodes.UnknownState, stateName);
            }

            s.AddStateCallback(method, false);
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
            if (_transiting)
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
                _transiting = true;

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
                    arc.CallTransitCallbacks();
                }

                CurrentState = target;
                target.CallEnterCallbacks();
                _transiting = false;

                if (_postedState.Length != 0)
                {
                    stateName = _postedState;
                    _postedState = string.Empty;
                    goto PostedStateRestart; //tail recursion
                }
            }
            catch
            {
                _transiting = false;
                throw;
            }
        }
    }
}
