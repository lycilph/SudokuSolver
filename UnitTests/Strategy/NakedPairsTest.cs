using Core.Model;
using Core.Model.Actions;
using Core.Strategy;

namespace UnitTests.Strategy;

// See more information in the NakedPairsStrategy class
public class NakedPairsTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = "4.....938.32.941...953..24.37.6.9..4529..16736.47.3.9.957..83....39..4..24..3.7.9";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = NakedPairsStrategy.ExecuteAndApply(p.Grid) as BaseSolveAction;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, result.Elements.Count);
    }
}
