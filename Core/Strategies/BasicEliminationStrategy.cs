using Core.Commands;
using Core.Models;

namespace Core.Strategies;

/* Strategy name: Basic Elimination
 * 
 * This removes a cells value from the candidates of all its peer cells
 * (Source: N/A)
 */

public class BasicEliminationStrategy : BaseStrategy<BasicEliminationStrategy>
{
    public override string Name => "Basic Elimination";

    public override ICommand? Plan(Grid grid)
    {
        var command = new BasicEliminationCommand(Name);

        foreach (var cell in grid.FilledCells())
        {
            var peers = cell.Peers.Where(p => p.IsEmpty && p.Candidates.Contains(cell.Value)).ToList();

            if (peers.Count > 0)
            {
                command.CellsToVisualize.Add(cell);
                command.Add(new CommandElement
                {
                    Description = $"Cell {cell.Index} eliminates candidate {cell.Value} from cell(s): {string.Join(',', peers.Select(c => c.Index))}",
                    Numbers = [cell.Value],
                    Cells = peers
                });
            }
        }

        return command.IsValid() ? command : null;
    }
}
