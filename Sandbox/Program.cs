using Core.Archive.DancingLinks;
using Core.Model;
using Core.Strategies;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        TestStrategySolver();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void TestDancingLinksSolver()
    {
        //var p = new Puzzle("4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9"); // 1 solution
        var p = new Puzzle("4......3..32..41...953..24.37.6.9..4.29..16.36.47.3.9...7..83....39..4..24....7.9"); // 24 solutions

        var results = DancingLinksSolver.Solve(p);

        Console.WriteLine($"{results.Count} solutions found");
        Console.WriteLine($"Execution Time: {p.Stats.ElapsedTime} ms");
        Console.WriteLine();

        Console.WriteLine("Solution(s):");
        foreach (var solution in results)
            Console.WriteLine(solution.ToSimpleString());
    }

    private static void TestStrategySolver()
    {
        var p = new Puzzle(".7.4.8.29..2.....4854.2...7..83742...2.........32617......936122.....4.313.642.7.");

        Console.WriteLine(p.Grid.ToString());
        Solver.Solve(p);

        Console.WriteLine($"Execution Time: {p.Stats.ElapsedTime} ms");
        Console.WriteLine($"Iterations run: {p.Stats.Iterations}");
        Console.WriteLine();

        foreach (var a in p.Actions)
            Console.WriteLine(a);

        if (p.IsSolved())
        {
            Console.WriteLine("Solved state");
            Console.WriteLine(p.Grid.ToString());
            Console.WriteLine(p.Grid.ToSimpleString());
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("NO SOLUTION FOUND");
            Console.WriteLine(p.Grid.ToString());
            Console.WriteLine();
        }
    }
}
