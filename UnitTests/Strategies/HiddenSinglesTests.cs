using Core.Commands;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace UnitTests.Strategies;

// See more information in the HiddenSinglesStrategy class
public class HiddenSinglesTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = ".28..7....16.83.7.....2.85113729.......73........463.729..7.......86.14....3..7..";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = HiddenSinglesStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Elements);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var input = ".6.8...4.....4...22.46....9..1..93...96...45...83.....1.7..32.59.2.5.....35..1.7.";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = HiddenSinglesStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Elements.Count);
    }
}
