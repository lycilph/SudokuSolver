using Core.Commands;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace UnitTests.Strategies;

// See more information in the NakedQuadsStrategy class
public class NakedQuadsTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = "....3..86....2..4..9..7852.3718562949..1423754..3976182..7.3859.392.54677..9.4132";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = NakedQuadsStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Elements.Count);
    }
}