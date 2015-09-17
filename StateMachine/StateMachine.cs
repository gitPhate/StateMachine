using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMachine.Exceptions;
using StateMachine.Invokers;
using StateMachine.EventArgs;

namespace StateMachine
{
    public delegate void StateCallback<TState>(TState currentState);
    public delegate void TransitionCallback<TState>(TState sourceState, TState targetState);

    public class StateMachine<TState>
    {
        private IDictionary<TState, StateContainer<TState>> _statesMap;
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

        public bool IsTransiting { get; private set; }
        public StateContainer<TState> CurrentState { get; private set; }

        public event EventHandler<TransitingEventArgs<TState>> Transiting;
        public event EventHandler<TransitedEventArgs<TState>> Transited;

        protected virtual void OnTransiting(TransitingEventArgs<TState> e)
        {
            if (Transiting != null)
            {
                Transiting(this, e);
            }
        }

        protected virtual void OnTransited(TransitedEventArgs<TState> e)
        {
            if (Transited != null)
            {
                Transited(this, e);
            }
        }

        public StateMachine()
        {
            _statesMap = new Dictionary<TState, StateContainer<TState>>();
            IsTransiting = false;

            CurrentState = null;
        }

        public void AddState(TState stateName)
        {
            if (this[stateName] != null)
            {
                throw new StateMachineException<TState>(ErrorCodes.AlreadyPresentState, stateName);
            }

            var state = new StateContainer<TState>(stateName);
            _statesMap[stateName] = state;
        }

        public void AddTransition(TState source, TState target)
        {
            var sourceState = this[source];

            if (sourceState == null)
            {
                throw new StateMachineException<TState>(ErrorCodes.UnknownState, source);
            }

            StateContainer<TState> targetState = this[target];

            if (targetState == null)
            {
                throw new StateMachineException<TState>(ErrorCodes.UnknownState, target);
            }

            sourceState.AddTransition(targetState);
        }

        public void AddEnterStateCallback(TState targetStateName, StateCallback<TState> method)
        {
            var targetState = this[targetStateName];

            if (targetState == null)
            {
                throw new StateMachineException<TState>(ErrorCodes.UnknownState, targetStateName);
            }

            targetState.AddStateCallback(method, true);
        }

        public void AddExitStateCallback(TState targetStateName, StateCallback<TState> method)
        {
            var targetState = this[targetStateName];

            if (targetState == null)
            {
                throw new StateMachineException<TState>(ErrorCodes.UnknownState, targetStateName);
            }

            targetState.AddStateCallback(method, false);
        }

        public void AddTransitionCallback(TState source, TState target, TransitionCallback<TState> method)
        {
            var state = this[source];

            if (state == null)
            {
                throw new StateMachineException<TState>(ErrorCodes.UnknownState, source);
            }

            state.AddTransitionCallback(target, method);
        }

        public void GoToState(TState stateName)
        {
            try
            {
                if (IsTransiting)
                {
                    throw new StateMachineException<TState>(ErrorCodes.AlreadyTransiting, stateName);
                }

                IsTransiting = true;
                OnTransiting(new TransitingEventArgs<TState>(stateName));
                var target = this[stateName];

                if (target == null)
                {
                    throw new StateMachineException<TState>(ErrorCodes.UnknownState, stateName);
                }

                if (CurrentState != null)
                {
                    var transition = CurrentState[target];

                    if (transition == null)
                    {
                        throw new StateMachineException<TState>(ErrorCodes.InvalidTransition, StateMachineException<TState>.MakeArcName(CurrentState.Name, target.Name));
                    }

                    CurrentState.CallExitCallbacks();
                    transition.CallTransitionCallbacks();
                }

                CurrentState = target;
                target.CallEnterCallbacks();
                IsTransiting = false;
                OnTransited(new TransitedEventArgs<TState>(CurrentState.Name));
            }
            catch
            {
                IsTransiting = false;
                throw;
            }
        }
    }
}
