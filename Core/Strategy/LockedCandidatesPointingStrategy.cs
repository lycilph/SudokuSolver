using Core.Model;
using Core.Model.Actions;
using Core.Model.Misc;

namespace Core.Strategy;

/* Strategy name: Locked Candidates (Pointing)
 * 
 * If any one number occurs twice or three times in just one unit (any row, column or box) 
 * then we can remove that number from the intersection of another unit. There are two types of intersection:
 * 
 *  1. A Pair or Triple in a box - if they are aligned on a row, n can be removed from the rest of the row.
 *  2. A Pair or Triple in a box - if they are aligned on a column, n can be removed from the rest of the column.
 *  
 * Test puzzle(s): 
 * .179.36......8....9.....5.7.72.1.43....4.2.7..6437.25.7.1....65....3......56.172. (Source: https://www.sudokuwiki.org/Intersection_Removal)
 * 93..5....2..63..95856..2.....318.57...5.2.98..8...5......8..1595.821...4...56...8 (Source: https://www.sudokuwiki.org/Intersection_Removal)
 */

public class LockedCandidatesPointingStrategy : BaseStrategy<LockedCandidatesPointingStrategy>
{
    public override string Name => "Locked Candidates (Pointing)";

    public override IPuzzleAction? Execute(Grid grid)
    {
        var action = new EliminationSolvePuzzleAction() { Description = Name };

        foreach (var box in grid.Boxes)
        {
            FindLockedCandidates(grid, box, grid.Rows, action);
            FindLockedCandidates(grid, box, grid.Columns, action);
        }

        if (action.HasElements())
            return action;
        else
            return null;
    }

    private void FindLockedCandidates(Grid grid, Unit box, Unit[] rows_or_columns, EliminationSolvePuzzleAction action)
    {
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
                var cells = unit.Cells.Except(box.Cells).Where(c => c.Has(value)).ToList();
                
                // Remove the value from all other cells in the column/row
                if (cells.Count > 0)
                    action.Add(new SolveActionElement()
                    {
                        Description = $"A pointing candidate {value} in {box.FullName} and {unit.FullName}, eliminates {value} in cells: {string.Join(',', cells.Select(c => c.Index))}",
                        Number = value,
                        Cells = cells
                    });
            }
        }
    }
}
