using Core.DancingLinks;
using Core.Infrastructure;
using Core.Model;

namespace Core;

public static class Generator
{
    public static Puzzle Generate(int initial_cells_filled = 20, int final_clues = 25)
    {
        var puzzle = new Puzzle();

        int initial_grid_tries = 0;
        while (!puzzle.IsSolved())
        {
            initial_grid_tries++;

            var grid = CreateInitialGrid(initial_cells_filled);
            if (grid != null)
                puzzle.Grid = grid;

            Console.WriteLine($"Generating initial grid (iteration {initial_grid_tries})");
        }

        var cells_to_remove = 81 - final_clues;
        for (int i = 0; i < cells_to_remove; i++)
        {
            RemoveRandomCell(puzzle.Grid, 10);
            Console.WriteLine($"Grid {puzzle.Grid.ToSimpleString()}");
            Console.WriteLine($"Clues #: {puzzle.Grid.FilledCells().Count()}");
        }

        return puzzle;
    }
    
    public static void RemoveRandomCell(Grid grid, int tries)
    {
        for (int i = 0; i < tries; i++)
        {
            var cell = grid.FilledCells().OrderBy(x => Random.Shared.Next()).First();
            var value = cell.Value;

            cell.Reset();
            var results = DancingLinksSolver.Solve(grid, true, 10);

            if (results.Count == 1)
            {
                return;
            }
            else
            {
                cell.Value = value;
                Console.WriteLine($"Oops, grid is no longer valid (iteration {i})");
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
