using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
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

            machine.AddTransitArcCallback("R", "G", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));
            machine.AddTransitArcCallback("R", "B", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));
            machine.AddTransitArcCallback("G", "R", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));
            machine.AddTransitArcCallback("G", "B", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));
            machine.AddTransitArcCallback("B", "R", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));
            machine.AddTransitArcCallback("B", "G", (sourceState, targetState) => Console.WriteLine("Going from {0} to {1}", sourceState, targetState));

            machine.GoToState("R");
            machine.GoToState("G");
            machine.GoToState("B");

            Console.ReadLine();
        }
    }
}
