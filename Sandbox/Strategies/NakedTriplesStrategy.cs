using Sandbox.Model;

namespace Sandbox.Strategies;

public class NakedTriplesStrategy : IStrategy
{
    public string Name => "Naked Triples";
    
    public static readonly NakedTriplesStrategy Instance = new ();

    public bool Step(Grid grid)
    {
        var found_triples = false;
        foreach (var unit in grid.AllUnits)
        {
            if (FindNakedTriples(unit))
                found_triples = true;
        }
        return found_triples;
    }

    private bool FindNakedTriples(Unit unit)
    {
        var found_triples = false;

        return found_triples;
    }

    public static bool Execute(Grid grid)
    {
        return Instance.Step(grid);
    }
}
