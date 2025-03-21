using Core.DancingLinks;
using Core.Extensions;
using Core.Models;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var input = ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4";
        var grid = new Grid().Load(input, true);

        (var solutions, var stats) = DancingLinksSolver.Solve(grid, true);
        Console.WriteLine(stats);
        Console.WriteLine($"Solutions found {solutions.Count}");
        foreach (var solution in solutions)
            Console.WriteLine(solution.ToSimpleString());

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
