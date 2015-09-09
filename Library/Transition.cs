﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.CallbackInvokers;
using Library.Exceptions;

namespace Library
{
    public class Transition<TState>
    {
        private IList<TransitionCallbackInvoker<TState>> _transitCallbacks;

        public StateContainer<TState> Source { get; private set; }
        public StateContainer<TState> Target { get; private set; }
        public bool IsUnlinked
        {
            get
            {
                return Source == null || Target == null;
            }
        }

        public Transition(StateContainer<TState> source, StateContainer<TState> target)
        {
            Source = source;
            Target = target;

            _transitCallbacks = new List<TransitionCallbackInvoker<TState>>();

            if (IsUnlinked)
            {
                throw new StateMachineException(ErrorCodes.InvalidArc, StateMachineException.MakeArcName(source.Name, target.Name));
            }
        }

        public void AddTransitionCallback(ArcCallback<TState> method)
        {
            TransitionCallbackInvoker<TState> aci = new TransitionCallbackInvoker<TState>(method);
            _transitCallbacks.Add(aci);
        }

        public void CallTransitionCallbacks()
        {
            foreach (TransitionCallbackInvoker<TState> aci in _transitCallbacks)
            {
                aci.Invoke(Source.Name, Target.Name);
            }
        }
    }
}
