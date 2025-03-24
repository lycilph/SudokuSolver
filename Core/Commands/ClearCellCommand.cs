using Core.Models;

namespace Core.Commands;

/// <summary>
/// This action is initiated in the UI by right clicking on a cell,
/// and will clear the cell of both the value and candidates
/// </summary>

public class ClearCellCommand(Cell cell) : ICommand
{
    public string Name { get; } = "Clear Cell";
    public string Description => ToString();

    private readonly Cell cell = cell;
    private int value;
    private IEnumerable<int> candidates = [];

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
        cell.Candidates.AddRange(candidates);
    }

    public override string ToString() => $"Clearing cell {cell.Index}";
}
