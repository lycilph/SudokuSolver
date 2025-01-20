using Sandbox.Model;

namespace Sandbox.Strategies;

public class HiddenTriplesStrategy : IStrategy
{
    public string Name => "Hidden Triples";

    public static readonly HiddenTriplesStrategy Instance = new();

    public bool Step(Grid grid)
    {
        throw new NotImplementedException();
    }

    public static bool Execute(Grid grid)
    {
        return Instance.Step(grid);
    }
}
