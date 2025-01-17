using Sandbox.Model;
using Sandbox.Strategies;
using System.Diagnostics;

namespace Sandbox;

/*
 * 1. First start by basic elimination of candidates
 * 2. Then find naked singles
 * 3. Then find hidden singles
 * 4. Then find naked pairs
 * 5. Bruteforce the rest
 */

public static class Solver
{
    private static readonly IStrategy[] strategies =
    [
        new BasicEliminationStrategy(),
        new NakedSinglesStrategy(),
        new HiddenSinglesStrategy(),
        new NakedPairsStrategy(),
        new HiddenPairsStrategy(),
    ];

    public static void Solve(Grid grid)
    {
        var iterations = 0;
        bool changed = true;

        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        Console.WriteLine($"Puzzle has {grid.EmptyCellsCount} empty cells");
        while (!grid.IsSolved && changed)
        {
            iterations++;

            changed = Step(grid);

            Console.WriteLine($"Iteration {iterations}: {grid.EmptyCellsCount} empty cells left and {grid.TotalCandidatesCount} candidates (changed {changed})");
        }

        Console.WriteLine(grid.IsSolved ? "Sudoku solved" : "Sudoku NOT solved!");

        stopwatch.Stop(); // Stop the stopwatch
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
    }

    private static bool Step(Grid grid)
    {
        foreach (var strategy in strategies)
        {
            if (strategy.Step(grid))
            {
                Console.WriteLine($" * {strategy.Name}");
                return true;
            }
        }

        return false;
    }
}

/*
    // Solved grid by brute force through backtracking
    public static void Backtrack(Grid grid)
    {
        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        BasicElimination(grid);

        if (SolveBacktrack(grid))
            Console.WriteLine("Sudoku solved by backtracking!");
        else
            Console.WriteLine("Sudoku not solved by backtracking!");

        stopwatch.Stop(); // Stop the stopwatch
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
    }

    private static bool SolveBacktrack(Grid grid)
    {
        if (grid.IsSolved)
            return true;

        var empty_cells = grid.Cells.Where(c => c.IsEmpty).OrderBy(c => c.CandidatesCount).ToArray();

        foreach (var cell in empty_cells)
        {
            var possible_values = cell.Candidates.ToArray();

            foreach (var value in possible_values)
            {
                // Check if value is used in any of the cells peers
                var is_valid = cell.Peers.All(p => p.Value != value);
                if (!is_valid)
                    continue;

                cell.Value = value;
                if (SolveBacktrack(grid))
                {
                    return true;
                }
                else
                {
                    cell.Value = 0;
                    cell.Candidates.UnionWith(possible_values);
                }
            }
            return false;
        }

        return false;
    }
}
*/