using Core.Model;

namespace Core.Algorithms;

/* Strategy name: Basic Elimination
 * 
 * This removes a cells value from the candidates of all its peer cells
 */

public class BasicEliminationStrategy : IStrategy
{
    public string Name => "Basic Elimination";

    public ISolveAction? Execute(Grid grid)
    {
        var action = new EliminationSolveAction() { Description = Name };

        foreach (var cell in grid.Cells.Where(c => c.HasValue))
        {
            var cells = cell.Peers.Where(p => p.IsEmpty && p.Candidates.Contains(cell.Value)).ToList();

            if (cells.Count > 0)
                action.Elements.Add(
                    new SolveActionElement()
                    {
                        Description = $"Cell {cell.Index} eliminates candidate {cell.Value} from: {string.Join(',', cells.Select(c => c.Index))}",
                        Number = cell.Value,
                        Cells = cells
                    });
        }

        if (action.Elements.Count != 0)
            return action;
        else
            return null;
    }
}
