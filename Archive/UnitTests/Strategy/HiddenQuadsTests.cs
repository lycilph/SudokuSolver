using Core.Model.Actions;
using Core.Model;
using Core.Strategy;

namespace UnitTests.Strategy;

// See more information in the HiddenQuadsStrategy class
public class HiddenQuadsTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = "65..87.24...649.5..4..25...57.438.61...5.1...31.9.2.85...89..1....213...13.75..98";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = HiddenQuadsStrategy.ExecuteAndApply(p.Grid) as BaseSolveAction;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Elements.Count);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var input = "9.15...46425.9..8186..1..2.5.2.......19...46.6.......2196.4.2532...6.817.....1694";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = HiddenQuadsStrategy.ExecuteAndApply(p.Grid) as BaseSolveAction;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(21, result.Elements.Count);
    }
}
