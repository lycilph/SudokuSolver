using Core.Commands;
using Core.Models;
using Core.Strategies;

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
