using Core.Commands;
using Core.Models;
using Core.Strategies;
using System.Diagnostics;

namespace Core;

public static class Solver
{
    public static readonly IStrategy[] KnownStrategies =
        [
            new BasicEliminationStrategy(),
            new NakedSinglesStrategy(),
            new HiddenSinglesStrategy(),
            new LockedCandidatesPointingStrategy(),
            new LockedCandidatesClaimingStrategy(),
            new NakedPairsStrategy(),
            new HiddenPairsStrategy(),
            new NakedTriplesStrategy(),
            new HiddenTriplesStrategy(),
            new NakedQuadsStrategy(),
            new HiddenQuadsStrategy(),
            new XWingStrategy(),
            new ChuteRemotePairsStrategy()
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
