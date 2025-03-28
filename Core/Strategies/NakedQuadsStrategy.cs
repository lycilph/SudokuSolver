using Core.Commands;
using Core.Models;

namespace Core.Strategies;

/* Strategy name: Naked Quads
 * 
 * Find quads where 4 cells share the same 4 candidates, and remove these candidates from the other cells in the unit
 * (Source: https://www.sudokuwiki.org/Naked_Candidates)
 * (Source: https://hodoku.sourceforge.net/en/tech_naked.php)
 * 
 * Test puzzle(s): 
 * ....3..86....2..4..9..7852.3718562949..1423754..3976182..7.3859.392.54677..9.4132 (Source: https://www.sudokuwiki.org/Naked_Candidates)
 */

public class NakedQuadsStrategy : BaseStrategy<NakedQuadsStrategy>
{
    public override string Name => "Naked Quads";
    public override int Difficulty => 8;

    public override ICommand? Plan(Grid grid)
    {
        var command = new NakedQuadsCommand(this);

        foreach (var unit in grid.AllUnits)
            FindNakedQuads(unit, command);

        return command.IsValid() ? command : null;
    }

    private void FindNakedQuads(Unit unit, NakedQuadsCommand command)
    {
        var empty_cells = unit.EmptyCells().ToArray();

        // If there are 4 or fewer cells left, it doesn't make sense to continue
        if (empty_cells.Length <= 4)
            return;

        // Since not all candidates need to be present in all the cells of the quad, we look for cells with 2, 3 or 4 candidates
        var candidates = empty_cells.Where(c => c.Count() >= 2 || c.Count() <= 4).ToArray();
        if (candidates.Length < 4)
            return;

        // Check the list for possible triples
        for (int i = 0; i < candidates.Length - 3; i++)
        {
            for (int j = i + 1; j < candidates.Length - 2; j++)
            {
                for (int k = j + 1; k < candidates.Length - 1; k++)
                {
                    for (int m = k + 1; m < candidates.Length; m++)
                    {
                        var quad = new Cell[] { candidates[i], candidates[j], candidates[k], candidates[m] };
                        var quad_candidates = quad.SelectMany(c => c.Candidates).Distinct().ToArray();

                        // If the 4 cells combined has 4 distinct candidates it is a naked quad
                        if (quad_candidates.Length == 4)
                            MarkEliminations(unit, quad, quad_candidates, command);
                    }
                }
            }
        }
    }

    private void MarkEliminations(Unit unit, Cell[] quad, int[] quad_candidates, NakedQuadsCommand command)
    {
        // Get all empty cells in the unit, that are not part of the triple
        var empty_cells = unit.Cells.Where(c => c.IsEmpty && !quad.Contains(c));

        // Mark the cells that contains any of the triple candidates for eliminiation
        foreach (var candidate in quad_candidates)
        {
            var cells = empty_cells.Where(c => c.Candidates.Contains(candidate)).ToList();
            if (cells.Count > 0)
                command.Add(new CommandElement
                {
                    Description = $"Naked quad of ({string.Join(',', quad_candidates)}) in {unit.FullName} in cells ({quad[0].Index},{quad[1].Index},{quad[2].Index},{quad[3].Index}) removes {candidate} from cell(s) {string.Join(',', cells.Select(c => c.Index))}",
                    Numbers = [candidate],
                    Cells = cells,
                    CellsToVisualize = quad.ToList()
                });
        }
    }
}
