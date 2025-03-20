namespace Core.Model.Actions;

/// <summary>
/// This action is initiated in the UI by left clicking on a cell with the input mode set to Digit,
/// and it will set the value of the cell and clear all candidates
/// </summary>
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

    public override string ToString()
    {
        return $"Settings cell {cell.Index} to {value}";
    }
}
