using Core.Model;

namespace Core.Algorithms;

/* Strategy name: Naked Pairs
 * 
 * Find pairs where 2 cells share the same 2 candidates, and remove these candidates from the other cells in the unit
 * (Source: https://www.sudokuwiki.org/Naked_Candidates)
 * (Source: https://hodoku.sourceforge.net/en/tech_naked.php)
 * 
 * Test puzzle(s): 
 * 4.....938.32.941...953..24.37.6.9..4529..16736.47.3.9.957..83....39..4..24..3.7.9 (Source: https://www.sudokuwiki.org/Naked_Candidates)
 */

public class NakedPairsStrategy : BaseStrategy<NakedPairsStrategy>
{
    public override string Name => "Naked Pairs";

    public override ISolveAction? Execute(Grid grid)
    {
        var action = new EliminationSolveAction() { Description = Name };

        foreach (var unit in grid.AllUnits)
        {
            var empty_cells = unit.EmptyCells();

            foreach (var pair in unit.FindNakedPairs())
            {
                // Since item1 and item2 in the pair are the same, any of them can be used here
                foreach (var candidate in pair.Item1.Candidates)
                {
                    // Find all cells that contain this candidate and mark them for elimination
                    var cells = empty_cells.Where(c => c != pair.Item1 && c != pair.Item2 && c.Candidates.Contains(candidate)).ToList();
                    if (cells.Count > 0)
                        action.Add(new SolveActionElement()
                        {
                            Description = $"Naked pair of {string.Join(',', pair.Item1.Candidates)} in {unit.FullName} in cells {pair.Item1.Index} and {pair.Item2.Index} removes {candidate} from cells {string.Join(',', cells.Select(c => c.Index))}",
                            Number = candidate,
                            Cells = cells
                        });
                }
            }
        }

        if (action.HasElements())
            return action;
        else
            return null;
    }
}
