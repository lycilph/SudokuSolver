using Sandbox.Model;
using Sandbox.Strategies;
using System.Diagnostics;

namespace Sandbox;

public static class Solver
{
    private static readonly IStrategy[] strategies =
    [
        new BasicEliminationStrategy(),
        new NakedSinglesStrategy(),
        new HiddenSinglesStrategy(),
        new LockedCandidatesPointing(),
        new LockedCandidatesClaiming(),
        new NakedPairsStrategy(),
        new NakedTriplesStrategy(),
        new NakedQuadsStrategy(),
        new HiddenPairsStrategy(),
        new HiddenTriplesStrategy(),
        new HiddenQuadsStrategy(),
    ];

    public static Grid Solve(Grid grid)
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

        if (!grid.IsSolved)
        {
            Console.WriteLine("Trying brute force solution");
            grid = BacktrackingSolver.Solve(grid);
        }

        return grid;
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