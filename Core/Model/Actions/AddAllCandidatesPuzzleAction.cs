namespace Core.Model.Actions;

public class AddAllCandidatesPuzzleAction : IPuzzleAction
{
    private readonly List<Cell> cells;
    private readonly Dictionary<Cell, IEnumerable<int>> candidates = [];

    public AddAllCandidatesPuzzleAction(IEnumerable<Cell> cells)
    {
        this.cells = [.. cells];
    }

    public void Do()
    {
        // This is needed if Do is called multiple times (ie. do->undo -> redo)
        candidates.Clear();

        foreach (var cell in cells)
        {
            candidates.Add(cell, cell.Candidates.ToArray());
            cell.AddAllCandidates();
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
}
