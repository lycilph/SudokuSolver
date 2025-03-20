using Core.Model;
using Core.Model.Actions;

namespace Core.Strategy;

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

    public override IPuzzleAction? Execute(Grid grid)
    {
        var action = new EliminationSolvePuzzleAction() { Description = Name };

        foreach (var unit in grid.AllUnits)
            FindNakedPairs(unit, action);

        if (action.HasElements())
            return action;
        else
            return null;
    }

    private void FindNakedPairs(Unit unit, EliminationSolvePuzzleAction action)
    {
        var pair_candidates = unit.Cells.Where(c => c.CandidatesCount() == 2).ToArray();

        for (int i = 0; i < pair_candidates.Length - 1; i++)
        {
            for (int j = i + 1; j < pair_candidates.Length; j++)
            {
                if (pair_candidates[i].Candidates.SetEquals(pair_candidates[j].Candidates))
                    MarkEliminations(unit, pair_candidates[i], pair_candidates[j], action);
            }
        }
    }

    private void MarkEliminations(Unit unit, Cell pair1, Cell pair2, EliminationSolvePuzzleAction action)
    {
        // Since item1 and item2 in the pair are the same, any of them can be used here
        foreach (var candidate in pair1.Candidates)
        {
            // Find all cells that contain this candidate and mark them for elimination
            var cells = unit.EmptyCells().Where(c => c != pair1 && c != pair2 && c.Candidates.Contains(candidate)).ToList();
            if (cells.Count > 0)
                action.Add(new SolveActionElement()
                {
                    Description = $"Naked pair of ({string.Join(',', pair1.Candidates)}) in {unit.FullName} in cells ({pair1.Index}, {pair2.Index}) removes {candidate} from cell(s) {string.Join(',', cells.Select(c => c.Index))}",
                    Number = candidate,
                    Cells = cells
                });
        }
    }
}
