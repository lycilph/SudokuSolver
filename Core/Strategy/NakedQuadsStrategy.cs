using Core.Model.Actions;
using Core.Model;

namespace Core.Strategy;

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

    public override IPuzzleAction? Execute(Grid grid)
    {
        var action = new EliminationSolvePuzzleAction() { Description = Name };

        //foreach (var unit in grid.AllUnits)
        //    FindNakedTriples(unit, action);

        if (action.HasElements())
            return action;
        else
            return null;
    }
}
