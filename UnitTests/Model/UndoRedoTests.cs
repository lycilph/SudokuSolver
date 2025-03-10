using Core.Model;
using Core.Strategy;

namespace UnitTests.Model;

public class UndoRedoTests
{
    [Fact]
    public void ApplySolverActionsTest()
    {
        // Arrange
        // This is a simple puzzle, solvable by only basic eliminations and naked singles
        var input = ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4";
        var solution = "652483917978162435314975628825736149791824563436519872269348751547291386183657294";

        // Act
        var p = new Puzzle(input);
        Solver.Solve(p);

        // Assert
        Assert.True(p.IsSolved());
        Assert.Equal(solution, p.Grid.ToSimpleString());
    }

    [Fact]
    public void ApplyAndUndoSolverActionsTest()
    {
        // Arrange
        // This is a simple puzzle, solvable by only basic eliminations and naked singles
        var input = ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4";
        var solution = "652483917978162435314975628825736149791824563436519872269348751547291386183657294";

        // Act
        var p = new Puzzle(input);
        Solver.Solve(p);

        // Make sure the puzzle is actually solved
        Assert.True(p.IsSolved());
        Assert.Equal(solution, p.Grid.ToSimpleString());

        // Undo all actions
        foreach (var a in p.Actions.AsEnumerable().Reverse())
            a.Undo();

        // Assert
        var undone = p.Grid.ToSimpleString();
        Assert.Equal(undone, input);
    }
}
