using Core.Strategies;

namespace Core.Commands;

/// <summary>
/// See LockedCandidatesPointingStrategy for more information
/// </summary>

public class LockedCandidatesPointingCommand(IStrategy strategy) : BaseCommand(strategy)
{
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
