using Sandbox.Model;
using Sandbox.Strategies;

namespace SolverTests;

public class LockedCandidatesPointingTests
{
    [Fact]
    public void Test1()
    {
        // https://www.sudoku9x9.com/techniques/lockedcandidates/
        var g = new Grid(".12...........3....56............................................................");

        BasicEliminationStrategy.Execute(g);
        LockedCandidatesPointing.Execute(g);

        //Assert.True(result);
    }
}
