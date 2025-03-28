using Core.Commands;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace UnitTests.Strategies;

// See more information in the SkyscraperStrategy class
public class SkyscraperTests
{
    [Fact]
    public void Test1()
    {
        // Arrange (skyscraper in columns)
        var input = "697.....2..1972.63..3..679.912...6.737426.95.8657.9.241486932757.9.24..6..68.7..9";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = SkyscraperStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Elements);
    }

    [Fact]
    public void Test2()
    {
        // Arrange (skyscraper in rows
        var input = "..1.28759.879.5132952173486.2.7..34....5..27.714832695....9.817.78.5196319..87524";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = SkyscraperStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Elements);
    }
}
