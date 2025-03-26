using Core.Commands;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace UnitTests.Strategies;

// See more information in the NakedTriplesStrategy class
public class NakedTriplesTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = ".7.4.8.29..2.....4854.2...7..83742...2.........32617......936122.....4.313.642.7.";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = NakedTriplesStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Elements.Count);
    }
}
