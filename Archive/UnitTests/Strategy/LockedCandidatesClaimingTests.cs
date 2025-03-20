using Core.Model.Actions;
using Core.Model;
using Core.Strategy;

namespace UnitTests.Strategy;

// See more information in the LockedCandidatesClaimingStrategy class
public class LockedCandidatesClaimingTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = ".16..78.3.9.8.....87...126..48...3..65...9.82.39...65..6.9...2..8...29369246..51.";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = LockedCandidatesClaimingStrategy.ExecuteAndApply(p.Grid) as BaseSolveAction;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Elements.Count);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var input = ".2.9437159.4...6..75.....4.5..48....2.....4534..352....42....81..5..426..9.2.85.4";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = LockedCandidatesClaimingStrategy.ExecuteAndApply(p.Grid) as BaseSolveAction;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, result.Elements.Count);
    }
}
