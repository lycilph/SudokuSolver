using Core.Archive.Model;

namespace Core.Archive.Strategies;

public class HiddenSinglesStrategy : IStrategy
{
    public string Name => "Hidden Singles";

    // Find cells where a value is only present as a candidate in one cell in a unit
    public bool Step(Grid grid, bool verbose = true)
    {
        var found_singles = false;
        foreach (var unit in grid.AllUnits)
        {
            if (FindHiddenSingles(unit, verbose))
                found_singles = true;
        }
        return found_singles;
    }

    private bool FindHiddenSingles(Unit unit, bool verbose)
    {
        var found_singles = false;

        var empty_cells = unit.Cells.Where(c => c.IsEmpty).ToArray();
        var possible_values = Enumerable.Range(1, 9).Except(unit.Cells.Where(c => c.HasValue).Select(c => c.Value)).ToArray();

        foreach (var value in possible_values)
        {
            var candidates = empty_cells.Where(c => c.Candidates.Contains(value)).ToArray();

            if (candidates.Length == 1)
            {
                candidates[0].Value = value;
                found_singles = true;

                if (verbose)
                    Console.WriteLine($" * Hidden single found in cell {candidates[0].Index} = {value} (in {unit.FullName})");
            }
        }

        return found_singles;
    }
}
