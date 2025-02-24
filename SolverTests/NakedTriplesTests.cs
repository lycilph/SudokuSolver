using Core.Algorithms;
using Core.Model;

namespace SolverTests;

// See more information in the NakedTriplesStrategy class
public class NakedTriplesTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = ".7.4.8.29..2.....4854.2...7..83742...2.........32617......936122.....4.313.642.7.";
        var p = new Puzzle(input);
        BasicEliminationStrategy.ExecuteAndApply(p.Grid);

        // Act
        var result = NakedTriplesStrategy.ExecuteAndApply(p.Grid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Elements.Count);
    }
}
