using Core.Model;
using Core.Model.Actions;
using System.Diagnostics;

namespace Core.Strategy;

public static class Solver
{
    private static readonly IStrategy[] strategies =
        [new BasicEliminationStrategy(),
        new NakedSinglesStrategy(),
        new HiddenSinglesStrategy()];

    public static void Solve(Puzzle puzzle)
    {
        puzzle.Stats.Reset();

        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        while (!puzzle.IsSolved())
        {
            IPuzzleAction? action = Step(puzzle);
            if (action == null)
                break;

            puzzle.Stats.Iterations++;

            puzzle.Actions.Add(action);
            action.Do();
        }

        stopwatch.Stop(); // Stop the stopwatch
        puzzle.Stats.ElapsedTime = stopwatch.ElapsedMilliseconds;
    }

    private static IPuzzleAction? Step(Puzzle puzzle)
    {
        IPuzzleAction? result = null;

        foreach (var strategy in strategies)
        {
            result = strategy.Execute(puzzle.Grid);
            if (result != null)
                return result;
        }

        return result;
    }
}
