using Core.Models;
using Core.Strategies;

namespace Core.Commands;

/// <summary>
/// See BasicEliminationStrategy for more information
/// </summary>

public class BasicEliminationCommand(IStrategy strategy) : BaseCommand(strategy)
{
    public List<Cell> CellsToVisualize { get; private set; } = [];

    public override void Do()
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
                cell.Candidates.Remove(element.Number);
    }

    public override void Undo()
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
                cell.Candidates.Add(element.Number);
    }
}
