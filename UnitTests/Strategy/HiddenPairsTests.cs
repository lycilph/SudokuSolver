using Core.Model;
using Core.Model.Actions;
using Core.Strategy;

namespace UnitTests.Strategy;

// See more information in the HiddenPairsStrategy class
public class HiddenPairsTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = "72.4.8.3..8.....474.1.768.281.739......851......264.8.2.968.41334......8168943275";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = HiddenPairsStrategy.ExecuteAndApply(p.Grid) as BaseSolveAction;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Elements.Count);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var input = ".49132....81479...327685914.96.518...75.28....38.46..5853267...712894563964513...";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = HiddenPairsStrategy.ExecuteAndApply(p.Grid) as BaseSolveAction;

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Elements);
    }
}
