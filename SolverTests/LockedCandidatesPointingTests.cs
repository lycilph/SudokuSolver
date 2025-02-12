using Core.Archive.Model;
using Core.Archive.Strategies;

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
        //..4...2132.13....9.3...247..2..6.7.......8..1.5.....3.6..2...9...25...6...5691...
        //Assert.True(result);
    }
}
