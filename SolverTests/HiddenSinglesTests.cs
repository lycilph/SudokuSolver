using Core.Algorithms;
using Core.Model;

namespace SolverTests;

public class HiddenSinglesTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = ".6.8...4.....4...22.46....9..1..93...96...45...83.....1.7..32.59.2.5.....35..1.7.";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = HiddenSinglesStrategy.ExecuteAndApply(p.Grid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Elements.Count);
    }
}
