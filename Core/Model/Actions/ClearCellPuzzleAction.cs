namespace Core.Model.Actions;

public class ClearCellPuzzleAction : IPuzzleAction
{
    private readonly Cell cell;
    private int value;
    private IEnumerable<int> candidates = [];

    public ClearCellPuzzleAction(Cell cell)
    {
        this.cell = cell;
    }

    public void Do()
    {
        value = cell.Value;
        candidates = cell.Candidates.ToArray();

        cell.Value = 0;
        cell.Candidates.Clear();
    }

    public void Undo()
    {
        cell.Value = value;
        cell.Candidates.UnionWith(candidates);
    }
}
