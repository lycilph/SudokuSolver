using Core.Commands;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace UnitTests.Strategies;

// See more information in the ChuteRemotePairsStrategy class
public class ChuteRemotePairsTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = ".2.9.56.39.5.3..12.631...5939..5..6.5.63.19.4.4.769385.3.5...9.859.13...614297538";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = ChuteRemotePairsStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Elements);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var input = "35.682.171.6573...7289413656..25.....1.43..5.5..168..4265814793...3965...3.725.8.";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = ChuteRemotePairsStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Elements);
    }
}
