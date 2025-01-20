using Sandbox.Model;

namespace Sandbox.Strategies;

public class LockedCandidatesClaiming : IStrategy
{
    public string Name => "Locked Candidates (Claiming)";

    public static readonly LockedCandidatesClaiming Instance = new();

    public bool Step(Grid grid)
    {
        bool found_locked_candidates = false;

        foreach (var box in grid.Boxes)
        {
            if (FindLockedCandidates(grid, box, grid.Columns))
                found_locked_candidates = true;

            if (FindLockedCandidates(grid, box, grid.Rows))
                found_locked_candidates = true;
        }

        return found_locked_candidates;
    }

    private bool FindLockedCandidates(Grid grid, Unit box, Unit[] rows_or_columns)
    {
        bool found_locked_candidates = false;

        // Make a dictionary mapping value to rows/columns which contain that value
        var value_to_rows_or_columns = new Dictionary<int, HashSet<int>>();

        foreach (var unit in rows_or_columns)
        {
            // If this column/row intersects the box under consideration, map values to columns/rows where they occur
            var cells = box.Cells.Intersect(unit.Cells, CellIndexComparer.Instance).ToArray();
            if (cells.Length > 0)
            {
                var possible_values = cells.SelectMany(c => c.Candidates).ToHashSet();
                foreach (var value in possible_values)
                    if (!value_to_rows_or_columns.ContainsKey(value))
                        value_to_rows_or_columns[value] = [unit.Index];
                    else
                        value_to_rows_or_columns[value].Add(unit.Index);
            }
        }

        // Check if any value is locked to a single column/row
        foreach (var value in value_to_rows_or_columns.Keys)
        {
            if (value_to_rows_or_columns[value].Count == 1)
            {
                var unit = rows_or_columns[value_to_rows_or_columns[value].First()];
                Console.WriteLine($" * Found a claiming candidate {value} in {box.FullName} and {unit.FullName}");

                // Remove the value from all other cells in the box
                foreach (var cell in box.Cells.Except(unit.Cells))
                {
                    if (cell.Candidates.Contains(value))
                    {
                        cell.Candidates.Remove(value);
                        found_locked_candidates = true;
                        Console.WriteLine($"   - Removed candidate {value} from {cell.Index}");
                    }
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
