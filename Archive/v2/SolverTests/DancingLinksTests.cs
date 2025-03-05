using Core.Archive.DancingLinks;
using Core.Model;

namespace SolverTests;

public class DancingLinksTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = "4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9";
        var p = new Puzzle(input);

        // Act
        var results = DancingLinksSolver.Solve(p);

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var input = "4......3..32..41...953..24.37.6.9..4.29..16.36.47.3.9...7..83....39..4..24....7.9";
        var p = new Puzzle(input);

        // Act
        var results = DancingLinksSolver.Solve(p);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(24, results.Count);
    }
}
