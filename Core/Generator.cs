using System.Diagnostics;
using Core.DancingLinks;
using Core.Extensions;
using Core.Models;

namespace Core;

public static class Generator
{
    public static (Grid, Statistics) Generate(int initial_cells_filled = 20, int final_clues = 25, int cell_removal_tries = 25)
    {
        var grid = new Grid();

        Stopwatch stopwatch = Stopwatch.StartNew();

        int initial_grid_tries = 0;
        while (!grid.IsSolved())
        {
            initial_grid_tries++;

            var temp = CreateInitialGrid(initial_cells_filled);
            if (temp != null)
                grid = temp;

            Console.WriteLine($"Generating initial grid (iteration {initial_grid_tries})");
        }
        Console.WriteLine($"Initial grid: {grid.ToSimpleString()}");

        var cells_to_remove = Grid.Size() - final_clues;
        for (int i = 0; i < cells_to_remove; i++)
        {
            RemoveRandomCell(grid, cell_removal_tries);
            Console.WriteLine($"Grid {grid.ToSimpleString()}");
            Console.WriteLine($"Clues #: {grid.FilledCells().Count()}");
        }

        stopwatch.Stop();
        
        // Fill in statistics
        var stats = new Statistics
        {
            Iterations = initial_grid_tries,
            ElapsedTime = stopwatch.ElapsedMilliseconds,
            CluesGiven = grid.FilledCells().Count()
        };

        return (grid, stats);
    }

    private static Grid? CreateInitialGrid(int initial_cells_filled)
    {
        var rnd_indices = Grid.AllCellIndices.OrderBy(x => Random.Shared.Next()).ToArray();
        var grid = new Grid();
        grid.FillCandidates();

        for (int i = 0; i < initial_cells_filled; i++)
        {
            var cell = grid[rnd_indices[i]];

            if (cell.Count() > 0)
                cell.Value = cell.Candidates.OrderBy(x => Random.Shared.Next()).First();
        }

        (var results, _) = DancingLinksSolver.Solve(grid, true, 10);
        if (results.Count > 0)
        {
            return results.OrderBy(x => Random.Shared.Next()).First();
        }
        else
            return null;
    }

    private static void RemoveRandomCell(Grid grid, int tries)
    {
        for (int i = 0; i < tries; i++)
        {
            // Clear a random cell (remember the value if it needs to be restored)
            var cell = grid.FilledCells().OrderBy(x => Random.Shared.Next()).First();
            var value = cell.Value;
            cell.Value = 0;
                        
            // The empty cells must also be reset again (otherwise it "remembers" the full solution)
            grid.EmptyCells().ForEach(c => 
            {
                c.Reset();
                c.FillCandidates();
            });

            (var results, _) = DancingLinksSolver.Solve(grid, true, 10);

            if (results.Count == 1)
            {
                return;
            }
            else
            {
                // Restore the cell, since it left the grid invalid
                cell.Value = value;
                cell.Clear();
                Console.WriteLine($"Oops, grid is no longer valid - {results.Count} solutions found (iteration {i})");
            }
        }
    }
}
