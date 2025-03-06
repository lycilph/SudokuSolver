namespace Core.Model.Actions;

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
        if (cell.HasCandidate(value))
            cell.RemoveCandidate(value);
        else
            cell.AddCandidate(value);
    }

    public void Undo()
    {
        if (cell.HasCandidate(value))
            cell.RemoveCandidate(value);
        else
            cell.AddCandidate(value);
    }
}
