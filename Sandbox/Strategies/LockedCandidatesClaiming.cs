using Sandbox.Model;

namespace Sandbox.Strategies;

// https://www.sudokusnake.com/claiming.php

public class LockedCandidatesClaiming : IStrategy
{
    public string Name => "Locked Candidates (Claiming)";

    public static readonly LockedCandidatesClaiming Instance = new();

    public bool Step(Grid grid, bool verbose = true)
    {
        bool found_locked_candidates = false;

        var units = grid.Rows.Concat(grid.Columns).ToArray();
        foreach (var box in grid.Boxes)
        {
            foreach (var unit in units)
            {
                if (FindLockedCandidates(grid, box, unit, verbose))
                    found_locked_candidates = true;
            }
        }

        return found_locked_candidates;
    }

    private bool FindLockedCandidates(Grid grid, Unit box, Unit unit, bool verbose)
    {
        bool found_locked_candidates = false;

        // Split cells into two groups: those in the box and those outside the box
        var cells_in_box = box.Cells.Intersect(unit.Cells).ToArray();
        var cells_outside_box = unit.Cells.Except(cells_in_box).ToArray();

        // Find the candidates for each of these groups
        var values_in_box = cells_in_box.SelectMany(cell => cell.Candidates).Distinct().ToArray();
        var values_outside_box = cells_outside_box.SelectMany(cell => cell.Candidates).Distinct().ToArray();

        // These values can be excluded from the cells in the box that are not in the row or column
        var values_only_in_box = values_in_box.Except(values_outside_box).ToArray();
        if (values_only_in_box.Length > 0 && verbose)
        {
            Console.WriteLine($" * Found claming candidates ({string.Join(',', values_only_in_box)}) in {box.FullName} and {unit.FullName}");
        }

        var cells_to_update = box.Cells.Except(unit.Cells).Where(c => c.IsEmpty).ToArray();
        foreach (var value in values_only_in_box)
        {
            foreach (var cell in cells_to_update)
            {
                if (cell.Candidates.Contains(value))
                {
                    cell.Candidates.Remove(value);
                    found_locked_candidates = true;
                    if (verbose)
                        Console.WriteLine($"   - Removed candidate {value} from {cell.Index}");
                }
            }
        }

        return found_locked_candidates;
    }

    public static bool Execute(Grid grid)
    {
        return Instance.Step(grid);
    }
}
