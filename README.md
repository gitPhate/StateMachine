# StateMachine
Simple StateMachine ported from C

Working example of configuration:
```C#
StateMachine machine = new StateMachine();

machine.AddState("R");
machine.AddState("G");
machine.AddState("B");

machine.AddArc("R", "G");
machine.AddArc("R", "B");
machine.AddArc("G", "R");
machine.AddArc("G", "B");
machine.AddArc("B", "R");
machine.AddArc("B", "G");

machine.AddEnterStateCallback("R", (currentState) => Console.WriteLine("Entering state {0}", currentState));
machine.AddEnterStateCallback("G", (currentState) => Console.WriteLine("Entering state {0}", currentState));
machine.AddEnterStateCallback("B", (currentState) => Console.WriteLine("Entering state {0}", currentState));

machine.AddExitStateCallback("R", (currentState) => Console.WriteLine("Exiting state {0}", currentState));
machine.AddExitStateCallback("G", (currentState) => Console.WriteLine("Exiting state {0}", currentState));
machine.AddExitStateCallback("B", (currentState) => Console.WriteLine("Exiting state {0}", currentState));

machine.AddTransitionCallback("R", "G", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));
machine.AddTransitionCallback("R", "B", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));
machine.AddTransitionCallback("G", "R", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));
machine.AddTransitionCallback("G", "B", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));
machine.AddTransitionCallback("B", "R", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));
machine.AddTransitionCallback("B", "G", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));

machine.GoToState("R");
machine.GoToState("G");
machine.GoToState("B");
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
