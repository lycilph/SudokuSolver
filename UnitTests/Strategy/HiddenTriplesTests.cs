using Core.Model.Actions;
using Core.Model;
using Core.Strategy;

namespace UnitTests.Strategy;

// See more information in the HiddenTriplesStrategy class
public class HiddenTriplesTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = ".....1.3.231.9.....65..31..6789243..1.3.5...6...1367....936.57...6.198433........";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = HiddenTriplesStrategy.ExecuteAndApply(p.Grid) as BaseSolveAction;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Elements.Count);
    }
}
