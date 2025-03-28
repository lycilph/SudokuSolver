using Core.Strategies;

namespace Core.Commands;

/// <summary>
/// See NakedSinglesStrategy for more information
/// </summary>

public class NakedSinglesCommand(IStrategy strategy) : BaseCommand(strategy)
{
    public override void Do()
    {
        foreach (var element in Elements)
        {
            var cell = element.Cell;
            var number = element.Number;

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
                cell.Candidates.Add(element.Number);
            }
    }
}
