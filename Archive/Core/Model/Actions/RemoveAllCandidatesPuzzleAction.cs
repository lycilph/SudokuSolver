namespace Core.Model.Actions;

/// <summary>
/// This action is initiated in the UI by clicking the "Clear Candidates" button
/// and it will remove all candidates in the list of cells given
/// </summary>
public class RemoveAllCandidatesPuzzleAction : IPuzzleAction
{
    private readonly List<Cell> cells;
    private readonly Dictionary<Cell, IEnumerable<int>> candidates = [];
    
    public RemoveAllCandidatesPuzzleAction(IEnumerable<Cell> cells)
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
            cell.ClearCandidates();
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
        return $"Removing all candidates from cells {string.Join(',', cells.Select(c => c.Index))}";
    }
}
