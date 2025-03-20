using Core.Model.Actions;
using Core.Model;
using Core.Strategy;

namespace UnitTests.Strategy;

// See more information in the NakedQuadsStrategy class
public class NakedQuadsTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = "....3..86....2..4..9..7852.3718562949..1423754..3976182..7.3859.392.54677..9.4132";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = NakedQuadsStrategy.ExecuteAndApply(p.Grid) as BaseSolveAction;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Elements.Count);
    }
}
