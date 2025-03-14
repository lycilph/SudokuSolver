using Core.Model;
using Core.Model.Actions;
using Core.Strategy;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var input = ".16..78.3.9.8.....87...126..48...3..65...9.82.39...65..6.9...2..8...29369246..51.";
        var p = new Puzzle(input);

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
