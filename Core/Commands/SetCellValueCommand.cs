using Core.Models;

namespace Core.Commands;

/// <summary>
/// This action is initiated in the UI by left clicking on a cell with the input mode set to Digit,
/// and it will set the value of the cell and clear all candidates
/// </summary>

public class SetCellValueCommand(Cell cell, int value) : ICommand
{
    public string Name { get; } = "Set Cell Value";
    public string Description => ToString();

    private readonly Cell cell = cell;
    private readonly int value = value;
    private IEnumerable<int> candidates = [];

    public void Do()
    {
        candidates = cell.Candidates.ToArray();
        cell.Value = value;
        cell.Candidates.Clear();
    }

    public void Undo()
    {
        cell.Value = 0;
        cell.Candidates.AddRange(candidates);
    }

    public override string ToString() => $"Settings cell {cell.Index} to {value}";
}
