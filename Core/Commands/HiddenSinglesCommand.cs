using Core.Models;
using Core.Strategies;

namespace Core.Commands;

/// <summary>
/// See HiddenSinglesStrategy for more information
/// </summary>

public class HiddenSinglesCommand(IStrategy strategy) : BaseCommand(strategy)
{
    private Dictionary<Cell, IEnumerable<int>> candidates = [];

    public override void Do()
    {
        // Clear potential previous values
        candidates.Clear();

        foreach (var element in Elements)
        {
            var cell = element.Cell;
            var number = element.Number;

            candidates.Add(cell, cell.Candidates.ToArray());
            cell.Value = number;
            cell.Clear();
        }
    }

    public override void Undo()
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
            {
                cell.Value = 0;
                cell.Candidates.Clear();
                cell.Candidates.AddRange(candidates[cell]);
            }
    }
}
