namespace Core.Model.Actions;

/// <summary>
/// This action is initiated by an indiviual strategy (possibly via the solver),
/// and will set the value in 1 or more cells
/// </summary>
public class ValueSolvePuzzleAction : BaseSolveAction
{
    public override void Do()
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
                cell.Value = element.Number;
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
