using Core.Algorithms;
using Core.Model;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        //var p = new Puzzle(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"); // Easy, only needs basic elimination and naked singles
        var p = new Puzzle("4.....938.32.941...953..24.37.6.9..4529..16736.47.3.9.957..83....39..4..24..3.7.9"); // Naked pairs

        Console.WriteLine(p.Grid.ToString());
        Solver.Solve(p);

        Console.WriteLine($"Execution Time: {p.Stats.ElapsedTime} ms");
        Console.WriteLine($"Iterations run: {p.Stats.Iterations}");
        Console.WriteLine();

        foreach (var a in p.Actions)
            Console.WriteLine(a);

        Console.WriteLine("Solved state");
        Console.WriteLine(p.Grid.ToString());
        Console.WriteLine();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
