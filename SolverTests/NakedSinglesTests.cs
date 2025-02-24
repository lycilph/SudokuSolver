using Core.Algorithms;
using Core.Model;

namespace SolverTests;

// See more information in the NakedSinglesStrategy class
public class NakedSinglesTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        // This is a simple puzzle, solvable by only basic eliminations and naked singles
        var input = ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = NakedSinglesStrategy.ExecuteAndApply(p.Grid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Elements.Count);
    }
}
