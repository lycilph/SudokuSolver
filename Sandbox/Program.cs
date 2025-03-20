using Core.Model;
using Core.Model.Actions;
using Core.Strategy;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var input = ".......9476.91..5..9...2.81.7..5..1....7.9....8..31.6724.1...7..1..9..459.....1..";
        var p = new Puzzle(input);

        //Console.WriteLine(p.Grid.ToString());

        //BasicEliminationStrategy.ExecuteAndApply(p.Grid);
        //Console.WriteLine(p.Grid.CandidatesToString());

        //var action = XWingStrategy.ExecuteAndApply(p.Grid);
        //Console.WriteLine(action);



        Solver.Solve(p);

        Console.WriteLine("Steps taken by solver:");
        foreach (var action in p.Actions.Cast<BaseSolveAction>())
        {
            Console.WriteLine(action.Description);
            foreach (var element in action.Elements)
                Console.WriteLine($" * {element.Description}");
        }
        Console.WriteLine();

        if (p.IsSolved())
            Console.WriteLine($"Puzzle was solved in {p.Actions.Count} steps (in {p.Stats.ElapsedTime} ms)");
        else
            Console.WriteLine($"No solution found");

        Console.WriteLine(p.Grid.ToString());
        Console.WriteLine(input);
        Console.WriteLine(p.Grid.ToSimpleString());

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
