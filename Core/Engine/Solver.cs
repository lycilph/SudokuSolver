using Core.Commands;
using Core.Models;
using Core.Strategies;
using System.Diagnostics;

namespace Core.Engine;

public static class Solver
{
    public static readonly IStrategy[] KnownStrategies =
        [
            new BasicEliminationStrategy(), // Difficulty 1
            new NakedSinglesStrategy(), // Difficulty 1
            new HiddenSinglesStrategy(), // Difficulty 2
            new LockedCandidatesPointingStrategy(), // Difficulty 3
            new LockedCandidatesClaimingStrategy(), // Difficulty 3
            new NakedPairsStrategy(), // Difficulty 4
            new HiddenPairsStrategy(), // Difficulty 5
            new NakedTriplesStrategy(), // Difficulty 6
            new HiddenTriplesStrategy(), // Difficulty 7
            new NakedQuadsStrategy(), // Difficulty 8
            new HiddenQuadsStrategy(), // Difficulty 9
            new XWingStrategy(), // Difficulty 10
            new SkyscraperStrategy(), // Difficulty 10
            new TwoStringKiteStrategy(), // Difficulty 10
            new ChuteRemotePairsStrategy()  // Difficulty 11
        ];

    public static (List<ICommand>, Statistics) Solve(Grid grid)
    {
        var stats = new Statistics();
        var commands = new List<ICommand>();

        Stopwatch stopwatch = Stopwatch.StartNew();

        while (!grid.IsSolved())
        {
            var command = Step(grid);
            if (command == null)
                break;

            stats.Iterations++;
            stats.IncrementStrategyCount(command.Name);

            commands.Add(command);
            command.Do();
        }

        stopwatch.Stop();

        stats.CluesGiven = grid.ClueCount();
        stats.ElapsedTime = stopwatch.ElapsedMilliseconds;

        return (commands, stats);
    }

    public static ICommand? Step(Grid grid)
    {
        return Step(grid, KnownStrategies);
    }

    public static ICommand? Step(Grid grid, IStrategy[] strategies)
    {
        ICommand? result = null;

        foreach (var strategy in strategies)
        {
            result = strategy.Plan(grid);
            if (result != null)
                return result;
        }

        return result;
    }
}
