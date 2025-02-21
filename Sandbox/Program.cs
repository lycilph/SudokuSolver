using Core.Algorithms;
using Core.Archive.DancingLinks;
using Core.Model;

namespace Sandbox;

internal class Program
{
    static void Main()
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

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void TestStrategySolver()
    {
        var p = new Puzzle("4.....938.32.941...953..24.37.6.9..4529..16736.47.3.9.957..83....39..4..24..3.7.9");

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
            Console.WriteLine("NO SOLUTION FOUND");
    }
}
