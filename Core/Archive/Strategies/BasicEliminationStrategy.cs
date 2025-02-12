using Core.Archive.Model;

namespace Core.Archive.Strategies;

public class BasicEliminationStrategy : IStrategy
{
    public string Name => "Basic Elimination";

    public static readonly BasicEliminationStrategy Instance = new();

    // Eliminate the cell value from the candidates of its peers
    public bool Step(Grid grid, bool verbose = true)
    {
        var candidate_count = grid.Cells.Sum(c => c.Candidates.Count);

        foreach (var cell in grid.Cells.Where(c => c.HasValue))
        {
            foreach (var peer in cell.Peers.Where(p => p.IsEmpty))
            {
                peer.Candidates.Remove(cell.Value);
            }
        }

        return candidate_count != grid.Cells.Sum(c => c.Candidates.Count);
    }

    public static bool Execute(Grid grid)
    {
        return Instance.Step(grid);
    }
}
