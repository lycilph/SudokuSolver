using Core;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        (var grid, var stats) = Generator.Generate();

        Console.WriteLine(stats);
        Console.WriteLine(grid.ToString());

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
