using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var g = new Grid().Load("697.....2..1972.63..3..679.912...6.737426.95.8657.9.241486932757.9.24..6..68.7..9", true);
        BasicEliminationStrategy.PlanAndExecute(g);
        var cmd = SkyscraperStrategy.PlanAndExecute(g);

        Console.WriteLine(cmd);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
