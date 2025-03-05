using Core.Model;

namespace Core.Strategies;

/* Strategy name: Naked Triples
 * 
 * Find triples where 3 cells share the same 3 candidates, and remove these candidates from the other cells in the unit
 * (Source: https://www.sudokuwiki.org/Naked_Candidates)
 * (Source: https://hodoku.sourceforge.net/en/tech_naked.php)
 * 
 * Test puzzle(s): 
 * .7.4.8.29..2.....4854.2...7..83742...2.........32617......936122.....4.313.642.7. (Source: https://www.sudokuwiki.org/Naked_Candidates)
 */

public class NakedTriplesStrategy : BaseStrategy<NakedTriplesStrategy>
{
    public override string Name => "Naked Triples";

    public override ISolveAction? Execute(Grid grid)
    {
        var action = new EliminationSolveAction() { Description = Name };

        foreach (var unit in grid.AllUnits)
            FindNakedTriples(unit, action);

        if (action.HasElements())
            return action;
        else
            return null;
    }

    private void FindNakedTriples(Unit unit, EliminationSolveAction action)
    {
        var empty_cells = unit.EmptyCells().ToArray();

        // If there are 3 or fewer cells left, it doesn't make sense to continue
        if (empty_cells.Length <= 3)
            return;

        // Since not all candidates need to be present in all the cells of the triple, we look for cells with 2 or 3 candidates
        var candidates = empty_cells.Where(c => c.CandidatesCount == 2 || c.CandidatesCount == 3).ToArray();
        if (candidates.Length < 3)
            return;

        // Check the list for possible triples
        for (int i = 0; i < candidates.Length - 2; i++)
        {
            for (int j = i + 1; j < candidates.Length - 1; j++)
            {
                for (int k = j + 1; k < candidates.Length; k++)
                {
                    var triple = new Cell[] { candidates[i], candidates[j], candidates[k] };
                    var triple_candidates = triple.SelectMany(c => c.Candidates).Distinct().ToArray();

                    // If the 3 cells combined has 3 distinct candidates it is a naked triple
                    if (triple_candidates.Length == 3)
                        MarkEliminations(unit, triple, triple_candidates, action);
                }
            }
        }
    }

    private void MarkEliminations(Unit unit, Cell[] triple, int[]triple_candidates, EliminationSolveAction action)
    {
        // Get all empty cells in the unit, that are not part of the triple
        var empty_cells = unit.Cells.Where(c => c.IsEmpty && !triple.Contains(c));
       
        // Mark the cells that contains any of the triple candidates for eliminiation
        foreach (var candidate in triple_candidates)
        {
            var cells = empty_cells.Where(c => c.Candidates.Contains(candidate)).ToList();
            if (cells.Count > 0)
                action.Add(new SolveActionElement()
                {
                    Description = $"Naked triple of ({string.Join(',', triple_candidates)}) in {unit.FullName} in cells ({triple[0].Index},{triple[1].Index},{triple[2].Index}) removes {candidate} from cell(s) {string.Join(',', cells.Select(c => c.Index))}",
                    Number = candidate,
                    Cells = cells
                });
        }
    }
}
