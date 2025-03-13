namespace Core.Model.Actions;

/// <summary>
/// This action is initiated in the UI by left clicking on a cell with the input mode set to Hint,
/// and will add or remove a single candidate to a cell
/// </summary>
public class ToggleCandidatePuzzleAction : IPuzzleAction
{
    private readonly Cell cell;
    private readonly int value;

    public ToggleCandidatePuzzleAction(Cell cell, int value)
    {
        this.cell = cell;
        this.value = value;
    }

    public void Do()
    {
        if (cell.Has(value))
            cell.Remove(value);
        else
            cell.Add(value);
    }

    public void Undo()
    {
        if (cell.Has(value))
            cell.Remove(value);
        else
            cell.Add(value);
    }

    public override string ToString()
    {
        return $"Toggling the candidate {value} in cell {cell.Index}";
    }
}
