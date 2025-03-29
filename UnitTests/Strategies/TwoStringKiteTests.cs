using Core.Commands;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace UnitTests.Strategies;

// See more information in the TwoStringKiteStrategy class
public class TwoStringKiteTests
{
    [Fact]
    public void Test1()
    {
        // Arrange (skyscraper in columns)
        var input = ".81.2.6...42.6..89.568..24.69314275842835791617568932451..3689223...846.86.2.....";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = TwoStringKiteStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Elements);
    }

    [Fact]
    public void Test2()
    {
        // Arrange (skyscraper in columns)
        var input = "3617..295842395671.5.2614831.8526.34625....18.341..5264..61.85258...2167216857349";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = TwoStringKiteStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Elements);
    }
}
