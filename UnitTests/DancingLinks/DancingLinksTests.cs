using Core.DancingLinks;
using Core.Extensions;
using Core.Models;

namespace UnitTests.DancingLinks;

public class DancingLinksTests
{
    [Fact]
    public void SimpleGridSingleSolution()
    {
        // Arrange
        var input = "4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9";
        var grid = new Grid().Load(input, true);

        // Act
        (var solutions, var stats) = DancingLinksSolver.Solve(grid);

        // Assert
        Assert.Equal(41, stats.CluesGiven);
        Assert.NotNull(solutions);
        Assert.Single(solutions);
    }

    [Fact]
    public void SimpleGridMultipleSolutions()
    {
        // Arrange
        var input = "4......3..32..41...953..24.37.6.9..4.29..16.36.47.3.9...7..83....39..4..24....7.9";
        var grid = new Grid().Load(input, true);

        // Act
        (var solutions, var stats) = DancingLinksSolver.Solve(grid);

        // Assert
        Assert.Equal(36, stats.CluesGiven);
        Assert.NotNull(solutions);
        Assert.Equal(24, solutions.Count);
    }

    [Fact]
    public void SimpleGridMaxSolutions()
    {
        // Arrange
        var input = "4......3..32..41...953..24.37.6.9..4.29..16.36.47.3.9...7..83....39..4..24....7.9";
        var grid = new Grid().Load(input, true);

        // Act
        (var solutions, var stats) = DancingLinksSolver.Solve(grid, true, 10);

        // Assert
        Assert.Equal(36, stats.CluesGiven);
        Assert.NotNull(solutions);
        Assert.Equal(10, solutions.Count);
    }
}