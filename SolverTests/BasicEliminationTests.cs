using Core.Model;
using Core.Strategies;

namespace SolverTests;

public class BasicEliminationTests
{
    [Fact]
    public void Test1()
    {
        var g = new Grid(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"); // Random grid
        var s = new BasicEliminationStrategy();

        var result = s.Step(g);

        Assert.True(result);
    }
}
