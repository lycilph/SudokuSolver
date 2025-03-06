namespace Core.Model.Actions;

public class SetValuePuzzleAction : IPuzzleAction
{
    private readonly Cell cell;
    private readonly int value;
    private IEnumerable<int> candidates = [];

    public SetValuePuzzleAction(Cell cell, int value)
    {
        this.cell = cell;
        this.value = value;
    }

    public void Do()
    {
        candidates = cell.Candidates.ToArray();
        cell.Value = value;
        cell.Candidates.Clear();
    }

    public void Undo()
    {
        cell.Value = 0;
        cell.Candidates.UnionWith(candidates);
    }
}
