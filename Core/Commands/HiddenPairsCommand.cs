using Core.Models;
using Core.Strategies;

namespace Core.Commands;

/// <summary>
/// See HiddenPairsStrategy for more information
/// </summary>

public class HiddenPairsCommand(IStrategy strategy) : BaseCommand(strategy)
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

            // Save the current candidates for this cell (but only if we have not already done so)
            if (!candidates.ContainsKey(cell))
                candidates.Add(cell, cell.Candidates.ToArray());

            cell.Remove(number);
        }
    }

    public override void Undo()
    {
        foreach (var pair in candidates)
        {
            var cell = pair.Key;
            var candidates = pair.Value;

            cell.Value = 0;
            cell.Candidates.Clear();
            cell.Candidates.AddRange(candidates);
        }
    }
}
