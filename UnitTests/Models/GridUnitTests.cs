using Core.Extensions;
using Core.Models;

namespace UnitTests.Models;

public class GridUnitTests
{
    [Fact]
    public void GridSetSucceedsTest()
    {
        // Arrange
        var input = "4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9";
        var grid = new Grid();

        // Act
        grid.Load(input);

        // Assert
        Assert.Equal(81, grid.Cells.Count());
        Assert.Equal(41, grid.ClueCount());
    }

    [Fact]
    public void GridSetThrowsOnTooLongStringTest()
    {
        // Arrange
        var input = "4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.";
        var grid = new Grid();

        // Act

        // Assert
        Assert.Throws<ArgumentException>(() => grid.Load(input));
    }

    [Fact]
    public void GridSetThrowsOnIllegalCharactersTest()
    {
        // Arrange
        var input = "4......38x32x941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9";
        var grid = new Grid();

        // Act

        // Assert
        Assert.Throws<ArgumentException>(() => grid.Load(input));
    }
}
