using Core.Model;
using Core.Model.Actions;

namespace Core.Strategy;

/* Strategy name: Locked Candidates (claiming)
 * 
 * If any one number occurs twice or three times in just one unit (any row, column or box) 
 * then we can remove that number from the intersection of another unit. There are two types of intersection:
 * 
 *  1. A Pair or Triple on a row - if they are all in the same box, n can be removed from the rest of the box.
 *  2. A Pair or Triple on a column - if they are all in the same box, n can be removed from the rest of the box.
 *  
 * Test puzzle(s): 
 * .16..78.3.9.8.....87...126..48...3..65...9.82.39...65..6.9...2..8...29369246..51. (Source: https://www.sudokuwiki.org/Intersection_Removal)
 * .2.9437159.4...6..75.....4.5..48....2.....4534..352....42....81..5..426..9.2.85.4 (Source: https://www.sudokuwiki.org/Intersection_Removal)
 */

public class LockedCandidatesClaimingStrategy : BaseStrategy<LockedCandidatesClaimingStrategy>
{
    public override string Name => "Locked Candidates (Claiming)";

    public override IPuzzleAction? Execute(Grid grid)
    {
        var action = new EliminationSolvePuzzleAction() { Description = Name };

        var units = grid.Rows.Concat(grid.Columns).ToArray();
        foreach (var box in grid.Boxes)
        {
            foreach (var unit in units)
                FindLockedCandidates(grid, box, unit, action);
        }

        if (action.HasElements())
            return action;
        else
            return null;
    }

    private void FindLockedCandidates(Grid grid, Unit box, Unit unit, EliminationSolvePuzzleAction action)
    {
        // Split cells into two groups: those in the box and those outside the box
        var cells_in_box = box.Cells.Intersect(unit.Cells).ToArray();
        var cells_outside_box = unit.Cells.Except(cells_in_box).ToArray();

        // Find the candidates for each of these groups
        var values_in_box = cells_in_box.SelectMany(cell => cell.Candidates).Distinct().ToArray();
        var values_outside_box = cells_outside_box.SelectMany(cell => cell.Candidates).Distinct().ToArray();

        // These values can be excluded from the cells in the box that are not in the row or column
        var values_only_in_box = values_in_box.Except(values_outside_box).ToArray();
        if (values_only_in_box.Length > 0)
        {
            foreach (var value in values_only_in_box)
            {
                // Find the cells where this candidate can be eliminated
                var cells_to_update = box.Cells.Except(unit.Cells).Where(c => c.IsEmpty && c.Has(value)).ToList();
                if (cells_to_update.Count > 0)
                    action.Add(new SolveActionElement()
                    {
                        Description = $"A claiming candidate {value} in in {box.FullName} and {unit.FullName}, eliminates {value} in cells: {string.Join(',', cells_to_update.Select(c => c.Index))}",
                        Number = value,
                        Cells = cells_to_update
                    });
            }
        }
    }
}
