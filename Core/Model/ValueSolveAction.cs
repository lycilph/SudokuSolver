
namespace Core.Model;

public class ValueSolveAction : BaseSolveAction 
{
    public override void Apply()
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
