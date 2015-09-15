using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMachine;
using StateMachine.EventArgs;

using NUnit.Framework;

namespace StateMachineTest
{
    public enum States
    {
        R,
        G,
        B
    }

    [TestFixture]
    public class MainTest
    {
        /*
         * TODO: This test is far from exhaustive.
         */

        [Test]
        public void Test1()
        {
            #region Creating the state machine instance called machine
            var machine = new StateMachine<States>();

            machine.AddState(States.R);
            machine.AddState(States.G);
            machine.AddState(States.B);

            machine.AddTransition(States.R, States.G);
            machine.AddTransition(States.G, States.B);
            #endregion

            string EnteringCallbackMsg = "", ExitingCallbackMsg = "", TransistionCallback = "";
            string TransitingEventMsg = "", TransitedEventMsg = "";

            #region Adding callbacks to machine
            Action<States> EnteringCallback = (state) => EnteringCallbackMsg = String.Format("Entering state {0}", state);
            machine.AddEnterStateCallback(States.R, EnteringCallback.Invoke);
            machine.AddEnterStateCallback(States.G, EnteringCallback.Invoke);
            machine.AddEnterStateCallback(States.B, EnteringCallback.Invoke);

            Action<States> ExitingCallback = (state) => ExitingCallbackMsg = String.Format("Exiting state {0}", state);
            machine.AddExitStateCallback(States.R, ExitingCallback.Invoke);
            machine.AddExitStateCallback(States.G, ExitingCallback.Invoke);
            machine.AddExitStateCallback(States.B, ExitingCallback.Invoke);

            Action<States, States> TransitionCallback = (from, to) => TransistionCallback = String.Format("Going from {0} to {1}", from, to);
            machine.AddTransitionCallback(States.R, States.G, TransitionCallback.Invoke);
            machine.AddTransitionCallback(States.G, States.B, TransitionCallback.Invoke);
            #endregion


            #region Adding events to machine
            machine.Transiting += new EventHandler<TransitingEventArgs<States>>((s, e) => TransitingEventMsg = String.Format("Event Fired: Transiting into state {0}", e.State));
            machine.Transited  += new EventHandler<TransitedEventArgs<States>>( (s, e) => TransitedEventMsg  = String.Format("Event Fired: Transited into state {0}", e.State));
            #endregion


            #region Testing machine
            Assert.AreEqual(machine.CurrentState, null);

            machine.GoToState(States.R);
            Assert.AreEqual(machine.CurrentState.Name, States.R);
            Assert.AreEqual(ExitingCallbackMsg, "");
            Assert.AreEqual(TransistionCallback, "");
            Assert.AreEqual(EnteringCallbackMsg, "Entering state R");
            Assert.AreEqual(TransitingEventMsg, "Event Fired: Transiting into state R");
            Assert.AreEqual(TransitedEventMsg, "Event Fired: Transited into state R");


            machine.GoToState(States.G);
            Assert.AreEqual(machine.CurrentState.Name, States.G);
            Assert.AreEqual(ExitingCallbackMsg, "Exiting state R");
            Assert.AreEqual(TransistionCallback, "Going from R to G");
            Assert.AreEqual(EnteringCallbackMsg, "Entering state G");
            Assert.AreEqual(TransitingEventMsg, "Event Fired: Transiting into state G");
            Assert.AreEqual(TransitedEventMsg, "Event Fired: Transited into state G");


            machine.GoToState(States.B);
            Assert.AreEqual(machine.CurrentState.Name, States.B);
            Assert.AreEqual(ExitingCallbackMsg, "Exiting state G");
            Assert.AreEqual(TransistionCallback, "Going from G to B");
            Assert.AreEqual(EnteringCallbackMsg, "Entering state B");
            Assert.AreEqual(TransitingEventMsg, "Event Fired: Transiting into state B");
            Assert.AreEqual(TransitedEventMsg, "Event Fired: Transited into state B");
            #endregion

        }
        
    }

}
