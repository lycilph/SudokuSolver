using Core.Models;

namespace Core.Commands;

/// <summary>
/// This action is initiated in the UI by left clicking on a cell with the input mode set to Hint,
/// and will add or remove a single candidate to a cell
/// </summary>

public class ToggleCellCandidateCommand(Cell cell, int value) : ICommand
{
    public string Name { get; } = "Toglle Cell Candidate";
    public string Description => ToString();

    private readonly Cell cell = cell;
    private readonly int value = value;

    public void Do()
    {
        if (cell.Contains(value))
            cell.Remove(value);
        else
            cell.Add(value);
    }

    public void Undo()
    {
        Do();
    }

    public bool IsValid() => true;
    public override string ToString() => $"Toggling the candidate {value} in cell {cell.Index}";
}
