using Core.Model;
using Core.Model.Actions;
using Core.Strategy;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var input = ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4";
        var p = new Puzzle(input);

        var temp = BasicEliminationStrategy.ExecuteAndApply(p.Grid);
        Console.WriteLine(temp);


        //Solver.Solve(p);

        //Console.WriteLine("Steps taken by solver:");
        //foreach (var action in p.Actions.Cast<BaseSolveAction>())
        //{
        //    Console.WriteLine(action.Description);
        //    foreach (var element in action.Elements)
        //        Console.WriteLine($" * {element.Description}");
        //}
        //Console.WriteLine();

        //if (p.IsSolved())
        //    Console.WriteLine($"Puzzle was solved in {p.Actions.Count} steps (in {p.Stats.ElapsedTime} ms)");
        //else
        //    Console.WriteLine($"No solution found");

        //Console.WriteLine(p.Grid.ToString());
        //Console.WriteLine(input);
        //Console.WriteLine(p.Grid.ToSimpleString());

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
