using Core;
using Core.Engine;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        (var grid, var grade) = Generator.Generate(11, 11, 25, 100);
        if (grid == null)
        {
            Console.WriteLine($"No grid found (last difficult {grade.Difficulty})");
        }
        else
        {
            Console.WriteLine(grid);
            Console.WriteLine(grade);
            Console.WriteLine(grid.ToSimpleString());
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
