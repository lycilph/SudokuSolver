using Core.Model;

namespace Core.Algorithms;

/* Strategy name: Naked Singles
 * 
 * Find cells with only one candidate, and set the value for that cell
 */

public class NakedSinglesStrategy : BaseStrategy<NakedSinglesStrategy>
{
    public override string Name => "Naked Singles";

    public override ISolveAction? Execute(Grid grid)
    {
        var action = new ValueSolveAction() { Description = Name };

        var cells = grid.Cells.Where(c => c.IsEmpty && c.Candidates.Count == 1).ToArray();
        foreach (var cell in cells)
        {
            action.Elements.Add(new SolveActionElement() 
            {
                Description = $"Naked single found in cell {cell.Index}",
                Number = cell.Candidates.First(),
                Cells = [cell]
            });
        }

        if (action.Elements.Count != 0)
            return action;
        else
            return null;
    }
}
