using Core.Model;

namespace SolverTests;

public class BasicEliminationTests
{
    [Fact]
    public void Test1()
    {
        // This is a simple puzzle, solvable by only basic eliminations and naked singles
        var input = ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4";
        var p = new Puzzle(input);

        //var g = new Grid(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"); // Random grid
        //var s = new BasicEliminationStrategy();

        //var result = s.Step(g);

        //Assert.True(result);
    }
}
