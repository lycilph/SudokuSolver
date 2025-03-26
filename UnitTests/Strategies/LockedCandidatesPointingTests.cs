using Core.Commands;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace UnitTests.Strategies;

// See more information in the LockedCandidatesPointingStrategy class
public class LockedCandidatesPointingTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = ".179.36......8....9.....5.7.72.1.43....4.2.7..6437.25.7.1....65....3......56.172.";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = LockedCandidatesPointingStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Elements.Count);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var input = "93..5....2..63..95856..2.....318.57...5.2.98..8...5......8..1595.821...4...56...8";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = LockedCandidatesPointingStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.Elements.Count);
    }
}
