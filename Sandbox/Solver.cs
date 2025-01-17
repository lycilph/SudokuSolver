using Sandbox.Model;

namespace Sandbox;

/*
 * 1. First start by basic elimination of candidates
 * 2. Then find naked singles
 * 3. Then find hidden singles
 * 4. Bruteforce the rest
 */

public static class Solver
{
    // Eliminate the cell value from the candidates of its peers
    public static void BasicElimination(Grid grid)
    {
        foreach (var cell in grid.Cells.Where(c => c.HasValue))
        {
            foreach (var peer in cell.Peers.Where(p => p.IsEmpty))
            {
                peer.Candidates.Remove(cell.Value);
            }
        }
    }

    // Find cells with only one candidate, and set the value for that cell
    public static int NakedSingles(Grid grid)
    {
        var cells = grid.Cells.Where(c => c.IsEmpty && c.Candidates.Count == 1).ToArray();
        foreach (var cell in cells)
        {
            cell.Value = cell.Candidates.First();
        }
        return cells.Length;
    }

    // Find cells where a value is only present as a candidate in one cell in a unit
    public static int HiddenSingles(Grid grid)
    {
        int count = 0;
        int unit_count = 0;

        foreach (var row in grid.Rows)
        {
            unit_count = HiddenSinglesInUnit(row.Cells);
            if (unit_count > 0)
                Console.WriteLine($"Hidden single found in row {row.Index}");
            count += unit_count;
        }

        foreach (var col in grid.Columns)
        {
            unit_count = HiddenSinglesInUnit(col.Cells);
            if (unit_count > 0)
                Console.WriteLine($"Hidden single found in column {col.Index}");
            count += unit_count;
        }

        foreach (var box in grid.Boxes)
        {
            unit_count = HiddenSinglesInUnit(box.Cells);
            if (unit_count > 0)
                Console.WriteLine($"Hidden single found in box {box.Index}");
            count += unit_count;
        }

        return count;
    }

    private static int HiddenSinglesInUnit(Cell[] cells)
    {
        int count = 0;

        var emptyCells = cells.Where(c => c.IsEmpty).ToArray();
        var possibleValues = Enumerable.Range(1, 9).Except(cells.Where(c => c.HasValue).Select(c => c.Value)).ToArray();

        foreach (var value in possibleValues)
        {
            var candidates = emptyCells.Where(c => c.Candidates.Contains(value)).ToArray();
            if (candidates.Length == 1)
            {
                candidates[0].Value = value;
                Console.WriteLine($"Found hidden single for cell {candidates[0].Index} with the value {value}");
                count++;
            }
        }

        return count;
    }
}
