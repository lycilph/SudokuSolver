using Core.Commands;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace UnitTests.Strategies;

// See more information in the XWingStrategy class
public class XWingTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = "1.....569492.561.8.561.924...964.8.1.64.1....218.356.4.4.5...169.5.614.2621.....5";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = XWingStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Elements.Count);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var input = ".......9476.91..5..9...2.81.7..5..1....7.9....8..31.6724.1...7..1..9..459.....1..";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = XWingStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Elements.Count);
    }
}
