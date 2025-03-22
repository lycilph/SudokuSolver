using Core.Models;

namespace Core.Commands;

/// <summary>
/// This action fills in all candidates to the list of cells given
/// </summary>

public class FillCandidatesCommand : ICommand
{
    public string Name { get; } = "Fill Candidates Command";
    public string Description => ToString();

    private readonly List<Cell> cells;
    private readonly Dictionary<Cell, IEnumerable<int>> candidates = [];

    public FillCandidatesCommand(IEnumerable<Cell> cells)
    {
        this.cells = [.. cells];
    }

    public void Do()
    {
        // This is needed if Do is called multiple times (ie. do -> undo -> redo)
        candidates.Clear();

        foreach (var cell in cells)
        {
            candidates.Add(cell, cell.Candidates.ToArray());
            cell.FillCandidates();
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

    public bool IsValid() => true;

    public override string ToString()
    {
        return $"Adding all candidates to cells: {string.Join(',', cells.Select(c => c.Index))}";
    }
}
