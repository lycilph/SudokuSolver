using Core.Archive.Model;

namespace Core.Archive.Strategies;

public class HiddenTriplesStrategy : IStrategy
{
    public string Name => "Hidden Triples";

    public static readonly HiddenTriplesStrategy Instance = new();

    public bool Step(Grid grid, bool verbose = true)
    {
        var found_triples = false;
        foreach (var unit in grid.AllUnits)
        {
            if (FindHiddenTriples(unit, verbose))
                found_triples = true;
        }
        return found_triples;
    }

    private bool FindHiddenTriples(Unit unit, bool verbose)
    {
        var found_triples = false;
        var values_to_candidates = new Dictionary<int, Cell[]>();
        var empty_cells = unit.Cells.Where(c => c.IsEmpty).ToArray();

        // Make a dictionary of values to cells that contain that value (but only if there are exactly 2 or 3 cells with that value)
        foreach (var value in Enumerable.Range(1, 9))
        {
            var cells_with_value = empty_cells.Where(c => c.Candidates.Contains(value)).ToArray();
            if (cells_with_value.Length >= 2)
                values_to_candidates[value] = cells_with_value;
        }

        for (int i = 1; i <= 7; i++)
        {
            for (int j = i + 1; j <= 8; j++)
            {
                for (var k = j + 1; k <= 9; k++)
                {
                    if (values_to_candidates.ContainsKey(i) &&
                        values_to_candidates.ContainsKey(j) &&
                        values_to_candidates.ContainsKey(k))
                    {
                        var union = values_to_candidates[i]
                            .Union(values_to_candidates[j])
                            .Union(values_to_candidates[k])
                            .Distinct()
                            .ToArray();

                        if (union.Length == 3)
                        {
                            foreach (var cell in union)
                            {
                                if (cell.Candidates.Except([i, j, k]).Any())
                                {
                                    cell.Candidates.IntersectWith([i, j, k]);
                                    found_triples = true;
                                    if (verbose)
                                        Console.WriteLine($"   - Removing triple ({i},{j},{k}) from cell {cell.Index}");
                                }
                            }
                            if (found_triples && verbose)
                                Console.WriteLine($" * Hidden triples ({i},{j},{k}): {string.Join(' ', union.Select(c => c.Index))} (in {unit.FullName})");
                        }
                    }
                }
            }
        }

        return found_triples;
    }

    public static bool Execute(Grid grid)
    {
        return Instance.Step(grid);
    }
}
