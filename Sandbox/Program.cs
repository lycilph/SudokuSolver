using Core.Algorithms;
using Core.Model;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var p = new Puzzle(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4");

        Console.WriteLine(p.Grid.ToString());
        Solver.Solve(p);

        Console.WriteLine($"Execution Time: {p.Stats.ElapsedTime} ms");
        Console.WriteLine($"Iterations run: {p.Stats.Iterations}");
        foreach (var a in p.Actions)
            Console.WriteLine(a);

        foreach (var a in p.Actions.AsEnumerable().Reverse())
            a.Undo(p.Grid);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
