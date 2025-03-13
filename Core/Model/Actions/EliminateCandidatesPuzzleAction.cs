namespace Core.Model.Actions;

/// <summary>
/// This action is used multiple places, both when placing a value in a cell and when filling in candidates.
/// It takes a list of cells, and the removes the values of these cells from their peers
/// </summary>
public class EliminateCandidatesPuzzleAction : IPuzzleAction
{
    private readonly List<Cell> cells;
    private readonly Dictionary<Cell, IEnumerable<int>> candidates = [];

    public EliminateCandidatesPuzzleAction(IEnumerable<Cell> cells)
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

            foreach (var peer in cell.Peers.Where(p => p.IsFilled))
                cell.Remove(peer.Value);
        }
    }

    public void Undo()
    {
        foreach (var cell in cells)
        {
            cell.Candidates.Clear();
            cell.Candidates.UnionWith(candidates[cell]);
        }
    }

    public override string ToString()
    {
        return $"Peer cell eliminating candidates from cells {string.Join(',', cells.Select(c => c.Index))}";
    }
}
