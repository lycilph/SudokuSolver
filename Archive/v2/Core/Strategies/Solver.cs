﻿using Core.Model;
using System.Diagnostics;

namespace Core.Strategies;

public static class Solver
{
    private static readonly IStrategy[] strategies = 
        [new BasicEliminationStrategy(),
         new NakedSinglesStrategy(),
         new HiddenSinglesStrategy(),
         new NakedPairsStrategy(),
         new HiddenPairsStrategy(),
         new NakedTriplesStrategy()];

    public static void Solve(Puzzle puzzle)
    {
        puzzle.Stats.Reset();

        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        while (!puzzle.IsSolved())
        {
            ISolveAction? action = Step(puzzle);
            if (action == null)
                break;

            puzzle.Stats.Iterations++;

            puzzle.Actions.Add(action);
            action.Apply();
        }

        stopwatch.Stop(); // Stop the stopwatch
        puzzle.Stats.ElapsedTime = stopwatch.ElapsedMilliseconds;
    }

    private static ISolveAction? Step(Puzzle puzzle)
    {
        ISolveAction? result = null;

        foreach (var strategy in strategies)
        {
            result = strategy.Execute(puzzle.Grid);
            if (result != null)
                return result;
        }

        return result;
    }
}
