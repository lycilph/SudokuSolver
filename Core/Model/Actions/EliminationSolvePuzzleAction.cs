namespace Core.Model.Actions;

/// <summary>
/// This action is initiated by an indiviual strategy (possibly via the solver),
/// and will remove 1 candidate values from 1 or more cells
/// </summary>
public class EliminationSolvePuzzleAction : BaseSolveAction
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
