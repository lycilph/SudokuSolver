using Core.Strategies;

namespace Core.Commands;

/// <summary>
/// See TwoStringKiteStrategy for more information
/// </summary>

public class TwoStringKiteCommand(IStrategy strategy) : BaseCommand(strategy)
{
    public override void Do()
    {
        foreach (var element in Elements)
        {
            var cell = element.Cell;
            var number = element.Number;

            cell.Candidates.Remove(number);
        }
    }

    public override void Undo()
    {
        foreach (var element in Elements)
        {
            var cell = element.Cell;
            var number = element.Number;

            cell.Candidates.Add(number);
        }
    }
}
