using Sandbox.Model;

namespace Sandbox.Strategies;

public class NakedSinglesStrategy : IStrategy
{
    public string Name => "Naked Singles";

    // Find cells with only one candidate, and set the value for that cell
    public bool Step(Grid grid, bool verbose = true)
    {
        var cells = grid.Cells.Where(c => c.IsEmpty && c.Candidates.Count == 1).ToArray();
        foreach (var cell in cells)
        {
            cell.Value = cell.Candidates.First();
            if (verbose)
                Console.WriteLine($" * Naked single found in cell {cell.Index}");
        }
        return cells.Length > 0;
    }
}
