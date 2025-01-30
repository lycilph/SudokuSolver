using Core.Model;

namespace Core.Strategies;

public class HiddenPairsStrategy : IStrategy
{
    public string Name => "Hidden Pairs";

    public bool Step(Grid grid, bool verbose = true)
    {
        var found_pairs = false;
        foreach (var unit in grid.AllUnits)
        {
            if (FindHiddenPairs(unit, verbose))
                found_pairs = true;
        }
        return found_pairs;
    }

    private bool FindHiddenPairs(Unit unit, bool verbose)
    {
        var found_pairs = false;
        var values_to_candidates = new Dictionary<int, Cell[]>();
        var empty_cells = unit.Cells.Where(c => c.IsEmpty).ToArray();

        // Make a dictionary of values to cells that contain that value (but only if there are exactly 2 cells with that value)
        foreach (var value in Enumerable.Range(1, 9))
        {
            var cells_with_value = empty_cells.Where(c => c.Candidates.Contains(value)).ToArray();
            if (cells_with_value.Length == 2)
                values_to_candidates[value] = cells_with_value;
        }

        for (int i = 1; i <= 8; i++)
        {
            for (int j = i + 1; j <= 9; j++)
            {
                if (values_to_candidates.ContainsKey(i) &&
                    values_to_candidates.ContainsKey(j) &&
                    values_to_candidates[i].SequenceEqual(values_to_candidates[j]))
                {
                    foreach (var cell in values_to_candidates[i])
                    {
                        if (cell.Candidates.Count > 2)
                        {
                            cell.Candidates.IntersectWith([i, j]);
                            found_pairs = true;
                        }
                    }
                    if (found_pairs && verbose)
                        Console.WriteLine($" * Hidden pairs ({i},{j}): {string.Join(' ', values_to_candidates[i].Select(c => c.Index))} (in {unit.FullName})");
                }
            }
        }

        return found_pairs;
    }
}
