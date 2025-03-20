using Core.Model;

namespace UnitTests.Model;

public class GridUnitTests
{
    [Fact]
    public void GridSetSucceedsTest()
    {
        // Arrange
        var input = "4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9";
        var grid = new Grid();

        // Act
        grid.Set(input);

        // Assert
        Assert.Equal(81, grid.Cells.Count());
    }

    [Fact]
    public void GridSetThrows1Test()
    {
        // Arrange
        var input = "4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.";
        var grid = new Grid();

        // Act

        // Assert
        Assert.Throws<ArgumentException>(() => grid.Set(input));
    }

    [Fact]
    public void GridSetThrows2Test()
    {
        // Arrange
        var input = "4......38x32x941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9";
        var grid = new Grid();

        // Act

        // Assert
        Assert.Throws<ArgumentException>(() => grid.Set(input));
    }
}
