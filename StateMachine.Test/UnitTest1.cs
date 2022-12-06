using StateMachine.Core;
using StateMachine.Core.EventArgs;
using System.Diagnostics;

namespace StateMachine.Test
{
    public class Tests
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
            [OneTimeSetUp]
            public void StartTest()
            {
                Trace.Listeners.Add(new ConsoleTraceListener());
            }

            [OneTimeTearDown]
            public void EndTest()
            {
                Trace.Flush();
            }

            /*
             * TODO: This test is far from exhaustive.
             */

            [Test]
            public void Test1()
            {
                #region Creating the state machine instance called machine
                StateMachine<States> machine = new StateMachine<States>();

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
                machine.Transiting += new EventHandler<TransitingEventArgs<States>>((s, e) => TransitingEventMsg = $"Event Fired: Transiting into state {e.State}");
                machine.Transited += new EventHandler<TransitedEventArgs<States>>((s, e) => TransitedEventMsg = $"Event Fired: Transited into state {e.State}");
                #endregion


                #region Testing machine
                Assert.AreEqual(machine.CurrentState, null);

                machine.GoToState(States.R);
                Assert.AreEqual(machine.CurrentState.State, States.R);
                Assert.AreEqual(ExitingCallbackMsg, "");
                Assert.AreEqual(TransistionCallback, "");
                Assert.AreEqual(EnteringCallbackMsg, "Entering state R");
                Assert.AreEqual(TransitingEventMsg, "Event Fired: Transiting into state R");
                Assert.AreEqual(TransitedEventMsg, "Event Fired: Transited into state R");


                machine.GoToState(States.G);
                Assert.AreEqual(machine.CurrentState.State, States.G);
                Assert.AreEqual(ExitingCallbackMsg, "Exiting state R");
                Assert.AreEqual(TransistionCallback, "Going from R to G");
                Assert.AreEqual(EnteringCallbackMsg, "Entering state G");
                Assert.AreEqual(TransitingEventMsg, "Event Fired: Transiting into state G");
                Assert.AreEqual(TransitedEventMsg, "Event Fired: Transited into state G");


                machine.GoToState(States.B);
                Assert.AreEqual(machine.CurrentState.State, States.B);
                Assert.AreEqual(ExitingCallbackMsg, "Exiting state G");
                Assert.AreEqual(TransistionCallback, "Going from G to B");
                Assert.AreEqual(EnteringCallbackMsg, "Entering state B");
                Assert.AreEqual(TransitingEventMsg, "Event Fired: Transiting into state B");
                Assert.AreEqual(TransitedEventMsg, "Event Fired: Transited into state B");
                #endregion

            }

            [Test]
            public void Example()
            {
                StateMachine<States> machine = new StateMachine<States>();
                StateMachineCallbacks callbacks = new StateMachineCallbacks();

                machine.AddState(States.R);
                machine.AddState(States.G);
                machine.AddState(States.B);

                machine.AddTransition(States.R, States.G);
                machine.AddTransition(States.R, States.B);
                machine.AddTransition(States.G, States.R);
                machine.AddTransition(States.G, States.B);
                machine.AddTransition(States.B, States.R);
                machine.AddTransition(States.B, States.G);

                machine.AddEnterStateCallback(States.R, callbacks.EnteringState);
                machine.AddEnterStateCallback(States.G, callbacks.EnteringState);
                machine.AddEnterStateCallback(States.B, callbacks.EnteringState);

                machine.AddExitStateCallback(States.R, callbacks.ExitingState);
                machine.AddExitStateCallback(States.G, callbacks.ExitingState);
                machine.AddExitStateCallback(States.B, callbacks.ExitingState);

                machine.AddTransitionCallback(States.R, States.G, callbacks.Transition);
                machine.AddTransitionCallback(States.R, States.B, callbacks.Transition);
                machine.AddTransitionCallback(States.G, States.R, callbacks.Transition);
                machine.AddTransitionCallback(States.G, States.B, callbacks.Transition);
                machine.AddTransitionCallback(States.B, States.R, callbacks.Transition);
                machine.AddTransitionCallback(States.B, States.G, callbacks.Transition);

                machine.Transiting += new EventHandler<TransitingEventArgs<States>>(callbacks.TransitingEventHandler);
                machine.Transited += new EventHandler<TransitedEventArgs<States>>(callbacks.TransitedEventHandler);

                try
                {
                    machine.GoToState(States.R);
                    machine.GoToState(States.G);
                    machine.GoToState(States.B);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }

                Console.ReadLine();
            }

            public class StateMachineCallbacks
            {
                public void EnteringState(States state)
                {
                    Console.WriteLine("Entering state {0}", state);
                }

                public void ExitingState(States state)
                {
                    Console.WriteLine("Exiting state {0}", state);
                }

                public void Transition(States from, States to)
                {
                    Console.WriteLine("Going from {0} to {1}", from, to);
                }

                public void TransitingEventHandler(object sender, TransitingEventArgs<States> e)
                {
                    Console.WriteLine("Event Fired: Transiting into state {0}", e.State);
                }

                public void TransitedEventHandler(object sender, TransitedEventArgs<States> e)
                {
                    Console.WriteLine("Event Fired: Transited into state {0}", e.State);
                }
            }
        }
    }
}