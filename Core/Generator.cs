using Core.DancingLinks;
using Core.Infrastructure;
using Core.Model;
using System.Diagnostics;

namespace Core;

public static class Generator
{
    public static Puzzle Generate(int initial_cells_filled = 20, int final_clues = 25, int cell_removal_tries = 25)
    {
        var puzzle = new Puzzle();

        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        int initial_grid_tries = 0;
        while (!puzzle.IsSolved())
        {
            initial_grid_tries++;

            var grid = CreateInitialGrid(initial_cells_filled);
            if (grid != null)
                puzzle.Grid = grid;

            //Console.WriteLine($"Generating initial grid (iteration {initial_grid_tries})");
        }

        var cells_to_remove = 81 - final_clues;
        for (int i = 0; i < cells_to_remove; i++)
        {
            RemoveRandomCell(puzzle.Grid, cell_removal_tries);
            //Console.WriteLine($"Grid {puzzle.Grid.ToSimpleString()}");
            //Console.WriteLine($"Clues #: {puzzle.Grid.FilledCells().Count()}");
        }

        puzzle.Stats.Iterations = initial_grid_tries;
        puzzle.Stats.ElapsedTime = stopwatch.ElapsedMilliseconds;
        puzzle.Stats.CluesGiven = puzzle.Grid.FilledCells().Count();

        return puzzle;
    }
    
    public static void RemoveRandomCell(Grid grid, int tries)
    {
        for (int i = 0; i < tries; i++)
        {
            // Clear a random cell (remember the value if it needs to be restored)
            var cell = grid.FilledCells().OrderBy(x => Random.Shared.Next()).First();
            var value = cell.Value;
            cell.Reset();

            // The empty cells must also be reset again (otherwise it "remembers" the full solution)
            grid.EmptyCells().ForEach(c => c.Reset());

            var results = DancingLinksSolver.Solve(grid, true, 10);

            if (results.Count == 1)
            {
                return;
            }
            else
            {
                // Restore the cell, since it left the grid invalid
                cell.Value = value;
                //Console.WriteLine($"Oops, grid is no longer valid - {results.Count} solutions found (iteration {i})");
            }
        }
    }

    public static Grid? CreateInitialGrid(int initial_cells_filled)
    {
        var rnd_indices = Grid.AllCellIndices.OrderBy(x => Random.Shared.Next()).ToArray();
        var grid = new Grid();
        grid.Cells.ForEach(c => c.Reset());

        for (int i = 0; i < initial_cells_filled; i++)
        {
            var cell = grid[rnd_indices[i]];

            if (cell.CandidatesCount() > 0)
                cell.Value = cell.Candidates.OrderBy(x => Random.Shared.Next()).First();
        }

        var results = DancingLinksSolver.Solve(grid, true, 10);
        if (results.Count > 0)
        {
            return results.OrderBy(x => Random.Shared.Next()).First();
        }
        else
            return null;
    }
}
