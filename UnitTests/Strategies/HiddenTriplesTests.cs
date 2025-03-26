using Core.Commands;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace UnitTests.Strategies;

// See more information in the HiddenTriplesStrategy class
public class HiddenTriplesTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = ".....1.3.231.9.....65..31..6789243..1.3.5...6...1367....936.57...6.198433........";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = HiddenTriplesStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Elements.Count);
    }
}
