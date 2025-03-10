using Core;
using Core.DancingLinks;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var puzzle = Generator.Generate(20, 20);
        Console.WriteLine(puzzle.Grid.ToSimpleString());
        Console.WriteLine(puzzle.Stats.ToString());
        Console.WriteLine($"{DancingLinksSolver.Solve(puzzle).Count} solutions found");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
