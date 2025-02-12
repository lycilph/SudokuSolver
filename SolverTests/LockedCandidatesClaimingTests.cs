using Core.Archive.Model;
using Core.Archive.Strategies;

namespace SolverTests;

public class LockedCandidatesClaimingTests
{
    [Fact]
    public void Test1()
    {
        // https://www.sudokusnake.com/claiming.php
        var g = new Grid("..4...2132.13....9.3...247..2..6.7.......8..1.5.....3.6..2...9...25...6...5691...");

        BasicEliminationStrategy.Execute(g);
        LockedCandidatesClaiming.Execute(g);
        
        //Assert.True(result);
    }
}
