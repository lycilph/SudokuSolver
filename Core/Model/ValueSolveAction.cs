
namespace Core.Model;

public class ValueSolveAction : BaseSolveAction 
{
    public override void Apply(Grid grid)
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
                cell.Value = element.Number;
    }

    public override void Undo(Grid grid)
    {
        throw new NotImplementedException();
    }
}
