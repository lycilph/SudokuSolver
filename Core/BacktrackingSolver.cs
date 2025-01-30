using Core.Model;
using System.Diagnostics;

namespace Core;

public class BacktrackingSolver
{
    public static Grid Solve(Grid grid)
    {
        int iterations = 0;
        Grid result = new();
        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        if (Step(grid, ref result, ref iterations))
            Console.WriteLine($"Sudoku solved by backtracking ({iterations} iterations)!");
        else
            Console.WriteLine("Sudoku not solved by backtracking!");

        stopwatch.Stop(); // Stop the stopwatch
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");

        return result;
    }

    private static bool Step(Grid grid, ref Grid result, ref int iterations)
    {
        if (grid.IsSolved)
        {
            result = grid;
            return true;
        }

        iterations++;
        var temp = new Grid(grid);

        // Basic Elimination
        foreach (var cell in temp.Cells.Where(c => c.HasValue))
        {
            foreach (var peer in cell.Peers.Where(p => p.IsEmpty))
            {
                peer.Candidates.Remove(cell.Value);
            }
        }

        // Naked Singles
        var cells = temp.Cells.Where(c => c.IsEmpty && c.Candidates.Count == 1).ToArray();
        foreach (var cell in cells)
        {
            cell.Value = cell.Candidates.First();
        }

        var empty_cells = temp.Cells.Where(c => c.IsEmpty).OrderBy(c => c.CandidatesCount).ToArray();
        foreach (var cell in empty_cells)
        {
            var possible_values = cell.Candidates.ToArray();

            foreach (var value in possible_values)
            {
                cell.Value = value;
                if (Step(temp, ref result, ref iterations))
                    return true;
            }
            return false;
        }
        return false;
    }
}
