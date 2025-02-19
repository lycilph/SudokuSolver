namespace Core.Model;

public class EliminationSolveAction : BaseSolveAction
{
    public override void Apply(Grid grid)
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
                cell.Candidates.Remove(element.Number);
    }

    public override void Undo(Grid grid)
    {
        throw new NotImplementedException();
    }
}
