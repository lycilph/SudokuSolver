using Core.Model;

namespace Core.Algorithms;

/* Strategy name: Basic Elimination
 * 
 * This removes a cells value from the candidates of all its peer cells
 * (Source: N/A)
 */

public class BasicEliminationStrategy : BaseStrategy<BasicEliminationStrategy>
{
    public override string Name => "Basic Elimination";

    public override ISolveAction? Execute(Grid grid)
    {
        var action = new EliminationSolveAction() { Description = Name };

        foreach (var cell in grid.Cells.Where(c => c.HasValue))
        {
            var peers = cell.Peers.Where(p => p.IsEmpty && p.Candidates.Contains(cell.Value)).ToList();

            if (peers.Count > 0)
                action.Add(
                    new SolveActionElement()
                    {
                        Description = $"Cell {cell.Index} eliminates candidate {cell.Value} from cells: {string.Join(',', peers.Select(c => c.Index))}",
                        Number = cell.Value,
                        Cells = peers
                    });
        }

        if (action.HasElements())
            return action;
        else
            return null;
    }
}
