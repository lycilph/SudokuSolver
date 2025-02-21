using Core.Algorithms;
using Core.Model;

namespace SolverTests;

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
        var result = HiddenPairsStrategy.ExecuteAndApply(p.Grid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Elements.Count);
    }
}
