# StateMachine
This little project was born from an old (but good) <a href="https://en.wikipedia.org/wiki/Finite-state_machine">state machine</a> written in C++, with the goal to improve it and make it object-oriented and suited for a .NET application.<br />
Special thanks to the [stateless](https://github.com/nblumhardt/stateless) project, that helped me pretty much.

## Events
Two event handlers have been implemented, **Transiting** and **Transited**. The **transiting** event fires *before* the exiting callbacks, while the **Transited** one is fired at the end of the process (*after* the entering callbacks).  

## Example
This is a working example of the configuration:
```C#
namespace Tester
{
    public enum States
    {
        R,
        G,
        B
    }

    class Program
    {
        static void Main(string[] args)
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
```

The output is:
```
Event Fired: Transiting into state R
Entering state R
Event Fired: Transited into state R
Event Fired: Transiting into state G
Exiting state R
Going from R to G
Entering state G
Event Fired: Transited into state G
Event Fired: Transiting into state B
Exiting state G
Going from G to B
Entering state B
Event Fired: Transited into state B
```
