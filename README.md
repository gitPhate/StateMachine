# StateMachine
This little project was born from an old (but good) <a href="https://en.wikipedia.org/wiki/Finite-state_machine">state machine</a> written in C++.<br />
My goal is to make it object-oriented and suitable for a .NET application.

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

            machine.GoToState(States.R);
            machine.GoToState(States.G);
            machine.GoToState(States.B);

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
            Console.WriteLine("Entering state {0}", state);
        }

        public void Transition(States from, States to)
        {
            Console.WriteLine("Going from {0} to {1}", from, to);
        }
    }
}
```

The output is:
```
Entering state R
Exiting state R
Going from R to G
Entering state G
Exiting state G
Going from G to B
Entering state B
```
