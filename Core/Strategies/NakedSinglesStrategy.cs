using Core.Commands;
using Core.Models;

namespace Core.Strategies;

/* Strategy name: Naked Singles
 * 
 * Find cells with only one candidate, and set the value for that cell
 * (Source: https://hodoku.sourceforge.net/en/tech_singles.php)
 * 
 * Test puzzle(s): 
 * .5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4 (Source: first puzzle in file puzzles0_kaggle)
 */

public class NakedSinglesStrategy : BaseStrategy<NakedSinglesStrategy>
{
    public override string Name => "Naked Singles";
    public override int Difficulty => 1;

    public override ICommand? Plan(Grid grid)
    {
        var command = new NakedSinglesCommand(this);

        var cells = grid.Cells.Where(c => c.IsEmpty && c.Candidates.Count == 1).ToArray();
        foreach (var cell in cells)
        {
            command.Add(new CommandElement
            {
                Description = $"Naked single {cell.Candidates.Single()} found in cell {cell.Index}",
                Numbers = [cell.Candidates.Single()],
                Cells = [cell]
            });
        }

        return command.IsValid() ? command : null;
    }
}
