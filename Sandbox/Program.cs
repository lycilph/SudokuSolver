using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var g = new Grid().Load("697.....2..1972.63..3..679.912...6.737426.95.8657.9.241486932757.9.24..6..68.7..9", true); // Skyscraper in columns
        //var g = new Grid().Load("..1.28759.879.5132952173486.2.7..34....5..27.714832695....9.817.78.5196319..87524", true); // Skyscraper in rows
        BasicEliminationStrategy.PlanAndExecute(g);
        var cmd = SkyscraperStrategy.PlanAndExecute(g);

        Console.WriteLine(cmd);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
