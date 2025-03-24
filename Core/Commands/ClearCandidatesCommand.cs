using Core.Models;

namespace Core.Commands;

/// <summary>
/// This action is initiated in the UI by clicking the "Clear Candidates" button
/// and it will remove all candidates in the list of cells given
/// </summary>

public class ClearCandidatesCommand(IEnumerable<Cell> cells) : ICommand
{
    public string Name { get; } = "Clear Candidates";
    public string Description => ToString();

    private readonly List<Cell> cells = [.. cells];
    private readonly Dictionary<Cell, IEnumerable<int>> candidates = [];

    public void Do()
    {
        // This is needed if Do is called multiple times (ie. do -> undo -> redo)
        candidates.Clear();

        foreach (var cell in cells)
        {
            candidates.Add(cell, cell.Candidates.ToArray());
            cell.Clear();
        }
    }

    public void Undo()
    {
        foreach (var cell in cells)
        {
            cell.Candidates.Clear();
            cell.Candidates.AddRange(candidates[cell]);
        }
    }

    public override string ToString() => $"Removing all candidates from cells {string.Join(',', cells.Select(c => c.Index))}";
}
