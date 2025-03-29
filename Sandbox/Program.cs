using Core;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        //var g = new Grid().Load(".81.2.6...42.6..89.568..24.69314275842835791617568932451..3689223...846.86.2.....", true); // 2-string kite in 5
        var g = new Grid().Load("3617..295842395671.5.2614831.8526.34625....18.341..5264..61.85258...2167216857349", true); // 2-string kite in 9
        BasicEliminationStrategy.PlanAndExecute(g);

        var command = TwoStringKiteStrategy.PlanAndExecute(g);
        Console.WriteLine(command);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
