using Core.Commands;
using Core.Extensions;
using Core.Models;
using Core.Strategies;

namespace UnitTests.Strategies;

// See more information in the NakedSinglesStrategy class
public class NakedSinglesTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        // This is a simple puzzle, solvable by only basic eliminations and naked singles
        var input = ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4";
        var grid = new Grid().Load(input, true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        // Act
        var result = NakedSinglesStrategy.PlanAndExecute(grid) as BaseCommand;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Elements.Count);
    }
}
