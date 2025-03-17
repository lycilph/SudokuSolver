using Core.Model;

namespace VisualStrategyDebugger.Temp;

public class BasicEliminationStrategy : IStrategy
{
    public string Name { get; } = "Basic Elimination";

    public IGridCommand? Plan(Grid grid)
    {
        var command = new BasicEliminationCommand();

        foreach (var cell in grid.FilledCells())
        {
            var peers = cell.Peers.Where(p => p.IsEmpty && p.Candidates.Contains(cell.Value)).ToList();

            if (peers.Count > 0)
                command.Elements.Add(new CommandElement
                {
                    Description = $"Cell {cell.Index} eliminates candidate {cell.Value} from cell(s): {string.Join(',', peers.Select(c => c.Index))}",
                    Number = cell.Value,
                    Cells = peers
                });
        }

        return command.Elements.Count > 0 ? command : null;
    }
}
