using Sandbox.Model;

namespace Sandbox.Strategies;

public class HiddenQuadsStrategy : IStrategy
{
    public string Name => "Hidden Quads";

    public static readonly HiddenQuadsStrategy Instance = new();

    public bool Step(Grid grid)
    {
        var found_quads = false;
        foreach (var unit in grid.AllUnits)
        {
            if (FindHiddenQuads(unit))
                found_quads = true;
        }
        return found_quads;
    }

    private bool FindHiddenQuads(Unit unit)
    {
        var found_quads = false;
        var values_to_candidates = new Dictionary<int, Cell[]>();
        var empty_cells = unit.Cells.Where(c => c.IsEmpty).ToArray();

        // Make a dictionary of values to cells that contain that value (but only if there are 2, 3 or 4 cells with that value)
        foreach (var value in Enumerable.Range(1, 9))
        {
            var cells_with_value = empty_cells.Where(c => c.Candidates.Contains(value)).ToArray();
            if (cells_with_value.Length >= 2)
                values_to_candidates[value] = cells_with_value;
        }
        for (int i = 1; i <= 6; i++)
        {
            for (int j = i + 1; j <= 7; j++)
            {
                for (var k = j + 1; k <= 8; k++)
                {
                    for (var l = k + 1; l <= 9; l++)
                    {
                        if (values_to_candidates.ContainsKey(i) &&
                            values_to_candidates.ContainsKey(j) &&
                            values_to_candidates.ContainsKey(k) &&
                            values_to_candidates.ContainsKey(l))
                        {
                            var union = values_to_candidates[i]
                                .Union(values_to_candidates[j])
                                .Union(values_to_candidates[k])
                                .Union(values_to_candidates[l])
                                .Distinct()
                                .ToArray();

                            if (union.Length == 4)
                            {
                                foreach (var cell in union)
                                {
                                    if (cell.Candidates.Except([i, j, k, l]).Any())
                                    {
                                        cell.Candidates.IntersectWith([i, j, k, l]);
                                        Console.WriteLine($"   - Removing quad ({i},{j},{k},{l}) from cell {cell.Index}");
                                        found_quads = true;
                                    }
                                }
                                if (found_quads)
                                    Console.WriteLine($" * Hidden quad ({i},{j},{k},{l}): {string.Join(' ', union.Select(c => c.Index))} (in {unit.FullName})");
                            }
                        }
                    }
                }
            }
        }

        return found_quads;
    }

    public static bool Execute(Grid grid)
    {
        return Instance.Step(grid);
    }
}
