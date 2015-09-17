# StateMachine

[![Build Status](https://travis-ci.org/sdenel/StateMachine.svg?branch=master)](https://travis-ci.org/sdenel/StateMachine)

This little project was born from an old (but good) <a href="https://en.wikipedia.org/wiki/Finite-state_machine">state machine</a> written in C++, with the goal to improve it and make it object-oriented and suited for a .NET application.<br />
Special thanks to the [stateless](https://github.com/nblumhardt/stateless) project, that helped me pretty much.

## Configuration
StateMachine has a simple workflow. It requires the definition of states, transitions between them and actions which can be performed when entering or exiting a state and when going from one to another.

The example below describes a three-states machine with trasitions from/to the next one.
<img src="http://i61.tinypic.com/raugrs.png" alt="state machine schema" width="600" height="400" />
```C#
enum States
{
    R,
    G,
    B
}

StateMachine<States> machine = new StateMachine<States>();

machine.AddState(States.R);
machine.AddState(States.G);
machine.AddState(States.B);

machine.AddTransition(States.R, States.G);
machine.AddTransition(States.R, States.B);
machine.AddTransition(States.G, States.R);
machine.AddTransition(States.G, States.B);
machine.AddTransition(States.B, States.R);
machine.AddTransition(States.B, States.G);

machine.AddEnterStateCallback(States.R, (state) => Console.WriteLine("Entering state {0}", state));
machine.AddEnterStateCallback(States.G, (state) => Console.WriteLine("Entering state {0}", state));
machine.AddEnterStateCallback(States.B, (state) => Console.WriteLine("Entering state {0}", state));

machine.AddExitStateCallback(States.R, (state) => Console.WriteLine("Exiting state {0}", state));
machine.AddExitStateCallback(States.G, (state) => Console.WriteLine("Exiting state {0}", state));
machine.AddExitStateCallback(States.B, (state) => Console.WriteLine("Exiting state {0}", state));

machine.AddTransitionCallback(States.R, States.G, (from, to) => Console.WriteLine("Going from {0} to {1}", from, to));
machine.AddTransitionCallback(States.R, States.B, (from, to) => Console.WriteLine("Going from {0} to {1}", from, to));
machine.AddTransitionCallback(States.G, States.R, (from, to) => Console.WriteLine("Going from {0} to {1}", from, to));
machine.AddTransitionCallback(States.G, States.B, (from, to) => Console.WriteLine("Going from {0} to {1}", from, to));
machine.AddTransitionCallback(States.B, States.R, (from, to) => Console.WriteLine("Going from {0} to {1}", from, to));
machine.AddTransitionCallback(States.B, States.G, (from, to) => Console.WriteLine("Going from {0} to {1}", from, to));
```

### StateMachine declaration
```C#
StateMachine<States> machine = new StateMachine<States>();
```
When declaring, StateMachine accepts an enum that indicates the states type.

### Adding logic
```C#
machine.AddState(States.R);
```
Call the `AddState()` method to add the state to the machine.

```C#
machine.AddTransition(States.R, States.G);
```
Call the `AddTransition()` method to add a new transition between two states.

### Adding actions
The actions performed by StateMachine can be defined using delegates. They will be kept in memory by StateMachine and executed when desired.<br />
```C#
machine.AddEnterStateCallback(States.R, (state) => Console.WriteLine("Entering state {0}", state));
machine.AddExitStateCallback(States.R, (state) => Console.WriteLine("Exiting state {0}", state));
machine.AddTransitionCallback(States.R, States.G, (from, to) => Console.WriteLine("Going from {0} to {1}", from, to));
```
The two state methods want the related state or states followed by the delegate. The first one accept the target state as a parameter, while the transition method wants the source state and the target one.

## Events

To keep track of what happens inside StateMachine, two event handlers have been implemented, **Transiting** and **Transited**.
```C#
machine.Transiting += new EventHandler<TransitingEventArgs<States>>(TransitingEventHandler);
machine.Transited += new EventHandler<TransitedEventArgs<States>>(callbacks.TransitedEventHandler);
...
private void TransitingEventHandler(object sender, TransitingEventArgs e)
{
    Console.WriteLine("Event fired: Transiting into state {0}", e.State);
}

private void TransitedEventHandler(object sender, TransitedEventArgs e)
{
    Console.WriteLine("Event fired: Transited into state {0}", e.State);
}

```
The **transiting** event fires *before* the exiting callbacks, while the **Transited** one is fired at the end of the process (*after* the entering callbacks).
