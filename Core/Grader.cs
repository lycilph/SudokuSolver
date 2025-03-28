using Core.Commands;
using Core.Extensions;
using Core.Models;

namespace Core;

public static class Grader
{
    public static Grade Grade(Grid grid)
    {
        var clone = new Grid().Load(grid.ToSimpleString(), true);
        (var commands, _) = Solver.Solve(clone);

        if (clone.IsSolved())
        {
            var base_commands = commands.Cast<BaseCommand>();
            foreach (var command in base_commands)
                Console.WriteLine($"Command {command.Name} difficulty {command.Difficulty}");

            var difficulty = base_commands.Max(s => s.Difficulty);
            var effort = base_commands.Sum(s => s.Difficulty);
            return new Grade(difficulty, effort);
        }

        return new Grade(100,0);
    }
}
