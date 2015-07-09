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

            machine.AddEnterStateCallback("R", (currentState, isEnter) => Console.WriteLine("Entering state {0}", currentState));
            machine.AddEnterStateCallback("G", (currentState, isEnter) => Console.WriteLine("Entering state {0}", currentState));
            machine.AddEnterStateCallback("B", (currentState, isEnter) => Console.WriteLine("Entering state {0}", currentState));

            machine.AddExitStateCallback("R", (currentState, isEnter) => Console.WriteLine("Exiting state {0}", currentState));
            machine.AddExitStateCallback("G", (currentState, isEnter) => Console.WriteLine("Exiting state {0}", currentState));
            machine.AddExitStateCallback("B", (x, y) => Console.WriteLine("Exiting state B"));

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

        public void EnterInAState(string currentState, bool isEnter)
        {
            Console.WriteLine("Entering state {0}", currentState);
        }

        public void ExitFromAState(string currentState, bool isEnter)
        {
            Console.WriteLine("Entering state {0}", currentState);
        }

        public void GoingToState(string sourceState, string targetState)
        {
            Console.WriteLine("Going from {0} to {1}", sourceState, targetState);
        }
    }
}
