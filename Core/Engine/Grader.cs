using Core.Commands;
using Core.Extensions;
using Core.Models;

namespace Core.Engine;

public static class Grader
{
    public static Grade Grade(Grid grid)
    {
        var clone = grid.Copy(true);
        (var commands, _) = Solver.Solve(clone);

        if (clone.IsSolved())
        {
            var base_commands = commands.Cast<BaseCommand>();

            var difficulty = base_commands.Max(s => s.Difficulty);
            var effort = base_commands.Sum(s => s.Difficulty);

            return new Grade(difficulty, effort);
        }

        return new Grade(100,0);
    }
}
