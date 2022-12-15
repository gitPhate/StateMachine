using StateMachine.Core;
using StateMachine.Core.EventArgs;
using StateMachine.Core.Exceptions;
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
            public async Task Test1()
            {
                #region Creating the state machine instance called machine
                StateMachine<States> machine = new StateMachine<States>(States.R);

                machine.AddState(States.G);
                machine.AddState(States.B);

                machine.AddArc(States.R, States.G);
                machine.AddArc(States.G, States.B);
                #endregion

                string EnteringCallbackMsg = "", ExitingCallbackMsg = "", TransistionCallback = "";
                string TransitingEventMsg = "", TransitedEventMsg = "";

                #region Adding callbacks to machine

                Func<States, Task> EnteringCallback = (state) => { EnteringCallbackMsg = String.Format("Entering state {0}", state); return Task.CompletedTask; };
                machine.AddEnterStateCallback(States.R, EnteringCallback);
                machine.AddEnterStateCallback(States.G, EnteringCallback);
                machine.AddEnterStateCallback(States.B, EnteringCallback);

                Func<States, Task> ExitingCallback = (state) => { ExitingCallbackMsg = String.Format("Exiting state {0}", state); return(Task)Task.CompletedTask; };
                machine.AddExitStateCallback(States.R, ExitingCallback);
                machine.AddExitStateCallback(States.G, ExitingCallback);
                machine.AddExitStateCallback(States.B, ExitingCallback);

                Func<States, States, Task> TransitionCallback = (from, to) => { TransistionCallback = String.Format("Going from {0} to {1}", from, to); return Task.CompletedTask; };
                machine.AddArcCallback(States.R, States.G, TransitionCallback);
                machine.AddArcCallback(States.G, States.B, TransitionCallback);
                #endregion


                #region Adding events to machine
                machine.Transiting += new EventHandler<TransitingEventArgs<States>>((s, e) => TransitingEventMsg = $"Event Fired: Transiting into state {e.State}");
                machine.Transited += new EventHandler<TransitedEventArgs<States>>((s, e) => TransitedEventMsg = $"Event Fired: Transited into state {e.State}");
                #endregion


                #region Testing machine
                Assert.That(machine.CurrentState, Is.EqualTo(States.R));

                await machine.GoToStateAsync(States.G).ConfigureAwait(false);
                Assert.That(machine.CurrentState, Is.EqualTo(States.G));
                Assert.That(ExitingCallbackMsg, Is.EqualTo("Exiting state R"));
                Assert.That(TransistionCallback, Is.EqualTo("Going from R to G"));
                Assert.That(EnteringCallbackMsg, Is.EqualTo("Entering state G"));
                Assert.That(TransitingEventMsg, Is.EqualTo("Event Fired: Transiting into state G"));
                Assert.That(TransitedEventMsg, Is.EqualTo("Event Fired: Transited into state G"));


                await machine.GoToStateAsync(States.B).ConfigureAwait(false);
                Assert.That(machine.CurrentState, Is.EqualTo(States.B));
                Assert.That(ExitingCallbackMsg, Is.EqualTo("Exiting state G"));
                Assert.That(TransistionCallback, Is.EqualTo("Going from G to B"));
                Assert.That(EnteringCallbackMsg, Is.EqualTo("Entering state B"));
                Assert.That(TransitingEventMsg, Is.EqualTo("Event Fired: Transiting into state B"));
                Assert.That(TransitedEventMsg, Is.EqualTo("Event Fired: Transited into state B"));
                #endregion

            }

            [Test]
            public async Task SimpleTest()
            {
                StateMachine<States> machine = CreateStateMachine();

                await machine.GoToStateAsync(States.G).ConfigureAwait(false);
                Assert.That(machine.CurrentState, Is.EqualTo(States.G));

                await machine.GoToStateAsync(States.B).ConfigureAwait(false);
                Assert.That(machine.CurrentState, Is.EqualTo(States.B));

                Assert.ThrowsAsync(
                    Is.TypeOf<StateMachineException<States>>()
                        .And.Property(nameof(StateMachineException<States>.ErrorCode)).EqualTo(ErrorCodes.ArcNotFound),
                    () => machine.GoToStateAsync(States.R)
                );
            }

            private static StateMachine<States> CreateStateMachine()
            {
                StateMachine<States> machine = new(States.R);

                machine.AddState(States.G);
                machine.AddState(States.B);

                machine.AddArc(States.R, States.G);
                machine.AddArc(States.G, States.B);

                machine.AddEnterStateCallback(States.R, StateMachineCallbacks.EnteringState);
                machine.AddEnterStateCallback(States.G, StateMachineCallbacks.EnteringState);
                machine.AddEnterStateCallback(States.B, StateMachineCallbacks.EnteringState);

                machine.AddExitStateCallback(States.R, StateMachineCallbacks.ExitingState);
                machine.AddExitStateCallback(States.G, StateMachineCallbacks.ExitingState);
                machine.AddExitStateCallback(States.B, StateMachineCallbacks.ExitingState);

                machine.AddArcCallback(States.R, States.G, StateMachineCallbacks.Transition);
                machine.AddArcCallback(States.G, States.B, StateMachineCallbacks.Transition);

                machine.Transiting += StateMachineCallbacks.TransitingEventHandler;
                machine.Transited += StateMachineCallbacks.TransitedEventHandler;

                return machine;
            }

            public class StateMachineCallbacks
            {
                public static Task EnteringState(States state)
                {
                    Console.WriteLine("Entering state {0}", state);
                    return Task.CompletedTask;
                }

                public static Task ExitingState(States state)
                {
                    Console.WriteLine("Exiting state {0}", state);
                    return Task.CompletedTask;
                }

                public static Task Transition(States from, States to)
                {
                    Console.WriteLine("Going from {0} to {1}", from, to);
                    return Task.CompletedTask;
                }

                public static void TransitingEventHandler(object? sender, TransitingEventArgs<States> e)
                {
                    Console.WriteLine("Event Fired: Transiting into state {0}", e.State);
                }

                public static void TransitedEventHandler(object? sender, TransitedEventArgs<States> e)
                {
                    Console.WriteLine("Event Fired: Transited into state {0}", e.State);
                }
            }
        }
    }
}