using Core.Model.Actions;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        IPuzzleAction action = new EliminationSolvePuzzleAction();
        action.Do();
    }
}
