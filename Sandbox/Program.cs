using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var g = new Grid().Load(".2.9.56.39.5.3..12.631...5939..5..6.5.63.19.4.4.769385.3.5...9.859.13...614297538", true);
        BasicEliminationStrategy.PlanAndExecute(g);
        var cmd = ChuteRemotePairsStrategy.PlanAndExecute(g);

        Console.WriteLine(cmd);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
