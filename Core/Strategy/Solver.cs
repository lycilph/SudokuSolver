using Core.Model;
using Core.Model.Actions;
using System.Diagnostics;

namespace Core.Strategy;

public static class Solver
{
    private static readonly IStrategy[] strategies =
        [
            new BasicEliminationStrategy(),
            new NakedSinglesStrategy(),
            new HiddenSinglesStrategy(),
            new LockedCandidatesPointingStrategy(),
            new LockedCandidatesClaimingStrategy()
        ];

    public static void Solve(Puzzle puzzle)
    {
        puzzle.Stats.Reset();

        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        while (!puzzle.IsSolved())
        {
            IPuzzleAction? action = Step(puzzle.Grid);
            if (action == null)
                break;

            puzzle.Stats.Iterations++;

            puzzle.Actions.Add(action);
            action.Do();
        }

        stopwatch.Stop(); // Stop the stopwatch
        puzzle.Stats.ElapsedTime = stopwatch.ElapsedMilliseconds;
    }

    public static IPuzzleAction? Step(Grid grid)
    {
        IPuzzleAction? result = null;

        foreach (var strategy in strategies)
        {
            result = strategy.Execute(grid);
            if (result != null)
                return result;
        }

        return result;
    }
}
