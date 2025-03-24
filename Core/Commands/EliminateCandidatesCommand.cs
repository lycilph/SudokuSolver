using Core.Models;

namespace Core.Commands;

/// <summary>
/// This action is used multiple places, both when placing a value in a cell and when filling in candidates.
/// It takes a list of cells, and then for each cell, gets all peers with values set, then removes these candidates
/// </summary>

public class EliminateCandidatesCommand : ICommand
{
    public string Name { get; } = "Eliminate Candidates";
    public string Description => ToString();

    private readonly List<Cell> cells;
    private readonly Dictionary<Cell, IEnumerable<int>> candidates = [];

    public EliminateCandidatesCommand(IEnumerable<Cell> cells)
    {
        this.cells = [.. cells];
    }

    public void Do()
    {
        // This is needed if Do is called multiple times (ie. do -> undo -> redo)
        candidates.Clear();

        foreach (var cell in cells)
        {
            // Save old state
            candidates.Add(cell, cell.Candidates.ToArray());

            // Get the values of all peer cells and remove these from the list of candidates of the current cell
            var peer_values = cell.Peers.Where(p => p.IsFilled).Select(p => p.Value).Distinct();
            cell.Candidates.RemoveRange(peer_values);
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

    public override string ToString() =>  $"Peer cell eliminating candidates from cells {string.Join(',', cells.Select(c => c.Index))}";
}
