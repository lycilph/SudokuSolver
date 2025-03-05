using Core.Model;

namespace UnitTests.Model;

public class CellUnitTests
{
    [Fact]
    public void CandidatesStringTest()
    {
        // Arrange
        var cell = new Cell(42);

        // Act
        cell.Candidates.Remove(2);
        cell.Candidates.Remove(4);

        // Assert
        Assert.Equal("1.3.56789", cell.GetCandidatesAsString());
    }

    [Fact]
    public void IndicesTests()
    {
        // Arrange
        var cell = new Cell(42);

        // Act
        // NA

        // Assert
        Assert.Equal(4, cell.Row);
        Assert.Equal(6, cell.Column);

        Assert.Equal(4, cell.GetIndexInUnit(UnitType.Row));
        Assert.Equal(6, cell.GetIndexInUnit(UnitType.Column));
    }
}
