using Core;
using Core.DancingLinks;
using Core.Model;
using Core.Strategy;
using System.Diagnostics;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var puzzle = Generator.Generate(20, 5);
        Console.WriteLine(puzzle.Grid.ToSimpleString());
        Console.WriteLine($"{DancingLinksSolver.Solve(puzzle).Count} solutions found");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private static void CreateSudokuPuzzle()
    {
        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        var grid = GenerateRandomGrid(new Grid());

        if (grid != null && grid.IsSolved())
        {
            for (int i = 0; i < 55; i++)
            {
                RemoveRandomCell(grid);
                Console.WriteLine($"Clues {grid.FilledCells().Count()}");
            }

            Console.WriteLine($"Final grid: {grid.ToSimpleString()}");
            Console.WriteLine($"Final clues #: {grid.FilledCells().Count()}");

            var results = DancingLinksSolver.Solve(grid);
            Console.WriteLine($"{results.Count} solutions");
        }
        else
            Console.WriteLine("No grid found, try again");

        Console.WriteLine($"Total elapsed time: {stopwatch.ElapsedMilliseconds} ms");
    }

    private static void RemoveRandomCell(Grid grid)
    {
        var cell = grid.FilledCells().OrderBy(x => Random.Shared.Next()).First();
        var value = cell.Value;

        cell.Reset();
        var results = DancingLinksSolver.Solve(grid, true, 10);

        if (results.Count != 1)
        {
            cell.Value = value;
            Console.WriteLine("Oops, grid is no longer valid");
        }
    }

    private static Grid? GenerateRandomGrid(Grid grid)
    {
        var rnd_indices = Grid.AllCellIndices.OrderBy(x => Random.Shared.Next()).ToArray();
        var cells_to_fill = 20;

        // Initialize the first cell
        grid[rnd_indices[0]].Value = Random.Shared.Next(1, 10);
        BasicEliminationStrategy.ExecuteAndApply(grid);

        // Select a random candidate for a random cell [cells_to_fill] times
        for (int i = 1; i < cells_to_fill; i++)
        {
            var cell = grid[rnd_indices[i]];

            if (cell.CandidatesCount() > 0)
            {
                var rnd_candidate = cell.Candidates.OrderBy(x => Random.Shared.Next()).First();
                cell.Value = rnd_candidate;
                BasicEliminationStrategy.ExecuteAndApply(grid);
            }
        }
        Console.WriteLine($"Random seed grid: {grid.ToSimpleString()}");

        var results = DancingLinksSolver.Solve(grid, true, 10);
        Console.WriteLine($"{results.Count} solutions found");

        if (results.Count > 0)
        {
            var result = results.OrderBy(x => Random.Shared.Next()).First();
            Console.WriteLine("Solution:");
            Console.WriteLine(result);

            return result;
        }

        return null;
    }
}
